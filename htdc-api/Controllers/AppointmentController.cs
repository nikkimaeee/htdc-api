using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using htdc_api.Authentication;
using htdc_api.Enumerations;
using htdc_api.Extensions;
using htdc_api.Interface;
using htdc_api.Models;
using htdc_api.Models.Payloads;
using htdc_api.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using Twilio.Rest.Content.V1;

namespace htdc_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentController: BaseController
{
    public AppointmentController(
        ApplicationDbContext context, 
        IMapper mapper, UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        IConfiguration configuration) : 
        base(context, mapper, userManager, emailSender, configuration)
    {
    }
    
    [HttpPost]
    [Route("GetAppointmentTime")]
    public async Task<IActionResult> GetAppointmentTime(AppointmentTimePayload payload)
    {
        try
        {
            var date = DateTime.Parse(payload.AppointmentDate);
            var list = await _context.AppointmentTimes.Where(x => x.IsActive).ToListAsync();
            var product = await _context.Products.FindAsync(payload.ServiceId);
            var duration = product.Duration;
            var results = new List<AppointmentTimeViewModel>();
            foreach (var item in list)
            {
                var remainingSlot = 1;
                var existingAppointment = await _context.AppointmentInformations
                    .Where(x => x.AppointmentDate.Date == date.Date
                                && x.IsActive && (x.Status == AppointmentStatusEnum.Approved || x.Status == AppointmentStatusEnum.Done))
                    .ToListAsync();

                existingAppointment = existingAppointment.Where(x => x.AppointmentTimeIds.Split(',')
                    .Select(int.Parse).ToList().Contains(item.Id)).ToList();
                
                if (existingAppointment.Any())
                {
                    remainingSlot = remainingSlot - existingAppointment.Count;
                }

                results.Add(new AppointmentTimeViewModel
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    AvailableSlot = remainingSlot,
                    MilitaryTime = item.MilitaryTime
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message + "/n/n" + ex.StackTrace);
        }
        
    }

    [HttpGet]
    [Route("GetDisabledDays")]
    public async Task<IActionResult> GetDisabledDays()
    {
        var dateToday = DateTime.UtcNow.ConvertTime();
        var timeSlots = await _context.AppointmentTimes.ToListAsync();

        var existingAppointments = await _context.AppointmentInformations
            .Where(x => x.IsActive && (x.Status == AppointmentStatusEnum.Approved || x.Status == AppointmentStatusEnum.Done)
            && x.AppointmentDate.Date > dateToday.Date)
            .Select(x => new
            {
                AppointmentDate = x.AppointmentDate,
                AppointmentTime = x.AppointmentTimeIds
            }).ToListAsync();


        var listOfDates = new List<string>();

        if (existingAppointments.Any())
        {
            var list = existingAppointments.GroupBy(x =>
                x.AppointmentDate.Date).ToList();
            
            foreach(var existingAppointment in list)
            {
                var isDisable = false;

                var result = existingAppointment.GroupBy(x => x.AppointmentTime).Select(x => new
                {
                    Count = x.Count(),
                    Id = x.Key
                }).ToList();

                foreach(var appointmentTime in timeSlots)
                {
                    var count = result.Count(x => x.Id.Split(',').Select(int.Parse).ToList().Contains(appointmentTime.Id));
                    if (count >= 1) {
                        isDisable = true;
                    }
                    else
                    {
                        isDisable = false;
                        break;
                    }
                }

                if(isDisable)
                {
                    listOfDates.Add(DateTime.Parse(existingAppointment.Key.ToString()).ToString("MM/dd/yyyy"));
                }
            }
        }

        return Ok(listOfDates.Distinct());
    }

    [HttpGet]
    [Route("GetPagedAppointments")]
    public async Task<IActionResult> GetPagedAppointments(int page, int rows, string status, DateTime dateFrom, DateTime dateTo, string? filter)
    {
        try
        {
            var statusEnum = (AppointmentStatusEnum)Enum.Parse(typeof(AppointmentStatusEnum), status);
            
            var appointmentList = new List<AppointmentTable>();
            var timeTable = await _context.AppointmentTimes.ToListAsync();
            var isAdmin = User.IsInRole("Admin");
            var totalRecords = 0;
            filter = filter != null ? filter.Trim() : null;
            if (isAdmin)
            {
                totalRecords = await _context.AppointmentInformations
                    .Join(_context.PatientInformations, x => x.PatientInformationId,y => y.Id,
                        (x, y) => new { PatientInformation = y, AppointmentInformation = x })
                    .Join(_context.Products, x => x.AppointmentInformation.ProductId, y => y.Id,
                        (x, y) => new { PatientInformation = x.PatientInformation, AppointmentInformation = x.AppointmentInformation, Product = y})
                    .Where(x => x.AppointmentInformation.IsActive &&
                        (filter == null || x.PatientInformation.FirstName.Contains(filter) || 
                        x.PatientInformation.LastName.Contains(filter) || 
                        x.PatientInformation.Email.Contains(filter) || 
                        x.AppointmentInformation.TransactionId.Contains(filter) ||
                        x.Product.Name.Contains(filter))
                        && x.AppointmentInformation.AppointmentDate.Date >= dateFrom.Date && x.AppointmentInformation.AppointmentDate.Date <= dateTo.Date
                        && (status == "All" || x.AppointmentInformation.Status == statusEnum)
                        )
                    .CountAsync();
                
                var list = await _context.AppointmentInformations
                    .Join(_context.PatientInformations, x => x.PatientInformationId,y => y.Id,
                        (x, y) => new { PatientInformation = y, AppointmentInformation = x })
                    .Join(_context.Products, x => x.AppointmentInformation.ProductId, y => y.Id,
                        (x, y) => new { PatientInformation = x.PatientInformation, AppointmentInformation = x.AppointmentInformation, Product = y })
                    .Where(x =>  x.AppointmentInformation.IsActive &&
                                (filter == null || 
                                x.PatientInformation.FirstName.Contains(filter) || 
                                x.PatientInformation.LastName.Contains(filter) || 
                                x.PatientInformation.Email.Contains(filter) || 
                                x.AppointmentInformation.TransactionId.Contains(filter) ||
                                x.Product.Name.Contains(filter))
                                && x.AppointmentInformation.AppointmentDate.Date >= dateFrom.Date && x.AppointmentInformation.AppointmentDate.Date <= dateTo.Date
                                && (status == "All" || x.AppointmentInformation.Status == statusEnum))
                    .OrderByDescending(x => x.AppointmentInformation.AppointmentDate)
                    .Skip((page - 1) * rows)
                    .Take(rows)
                    .ToListAsync(); 
                
                foreach (var appointment in list)
                {
                    var timeIds =  appointment.AppointmentInformation.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
                    var selectedSlots = timeTable
                        .Where(x => appointment.AppointmentInformation.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                        .OrderBy(x => x.MilitaryTime).ToList();
                    var item = new AppointmentTable
                    {
                        Id = appointment.AppointmentInformation.Id,
                        Product = await _context.Products.FindAsync(appointment.AppointmentInformation.ProductId),
                        PatientInformation = appointment.PatientInformation,
                        PaymentMethod = appointment.AppointmentInformation.PaymentMethod,
                        AspNetUserId = appointment.AppointmentInformation.AspNetUserId,
                        AppointmentTime = selectedSlots,
                        AppointmentTimeLabel = selectedSlots.Count > 1
                            ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[selectedSlots.Count - 1].Name.Split("-")[0].Trim()}"
                            : selectedSlots[0].Name,
                        AppointmentDate = appointment.AppointmentInformation.AppointmentDate,
                        IsPaid = appointment.AppointmentInformation.IsPaid,
                        Status = appointment.AppointmentInformation.Status.GetDisplayName(),
                        IsWalkIn = appointment.AppointmentInformation.IsWalkIn,
                        TransactionId = appointment.AppointmentInformation.TransactionId,
                        ReferenceNumber = appointment.AppointmentInformation.ReferenceNumber,
                        IsPwd = appointment.AppointmentInformation.IsPwd,
                        IsPregnant = appointment.AppointmentInformation.IsPregnant,
                        IsSenior = appointment.AppointmentInformation.IsSenior
                    };
                    
                    if (!string.IsNullOrEmpty(appointment.AppointmentInformation.MedCert) && System.IO.File.Exists(appointment.AppointmentInformation.MedCert))
                    {
                        byte[] imageArray = System.IO.File.ReadAllBytes(appointment.AppointmentInformation.MedCert);
                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                        item.MedCert = base64ImageRepresentation;
                    }
                    else
                    {
                        item.MedCert = string.Empty;
                    }
                    appointmentList.Add(item);
                }
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                totalRecords = await _context.AppointmentInformations
                    .Join(_context.PatientInformations, x => x.PatientInformationId,y => y.Id,
                        (x, y) => new { PatientInformation = y, AppointmentInformation = x })
                    .Join(_context.Products, x => x.AppointmentInformation.ProductId, y => y.Id,
                        (x, y) => new { PatientInformation = x.PatientInformation, AppointmentInformation = x.AppointmentInformation, Product = y })
                    .Where(x => 
                        x.AppointmentInformation.IsActive && x.AppointmentInformation.AspNetUserId == userId && 
                                (filter == null || 
                                x.PatientInformation.FirstName.Contains(filter) || 
                                x.PatientInformation.LastName.Contains(filter) || 
                                x.PatientInformation.Email.Contains(filter) || 
                                x.AppointmentInformation.TransactionId.Contains(filter) ||
                                x.Product.Name.Contains(filter))
                        && (status == "All" || x.AppointmentInformation.Status == statusEnum))
                    .CountAsync();
                var list = await _context.AppointmentInformations
                    .Join(_context.PatientInformations, x => x.PatientInformationId,y => y.Id,
                        (x, y) => new { PatientInformation = y, AppointmentInformation = x })
                    .Join(_context.Products, x => x.AppointmentInformation.ProductId, y => y.Id,
                        (x, y) => new { PatientInformation = x.PatientInformation, AppointmentInformation = x.AppointmentInformation, Product = y })
                    .Where(x => 
                        x.AppointmentInformation.IsActive && x.AppointmentInformation.AspNetUserId == userId && 
                                (filter == null || x.PatientInformation.FirstName.Contains(filter) || 
                                x.PatientInformation.LastName.Contains(filter) || 
                                x.PatientInformation.Email.Contains(filter) || 
                                x.AppointmentInformation.TransactionId.Contains(filter) ||
                                x.Product.Name.Contains(filter))
                        && (status == "All" || x.AppointmentInformation.Status == statusEnum))
                    .OrderByDescending(x => x.AppointmentInformation.AppointmentDate)
                    .Skip((page - 1) * rows)
                    .Take(rows)
                    .ToListAsync();
                foreach (var appointment in list)
                {
                    var timeIds =  appointment.AppointmentInformation.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
                    var selectedSlots = timeTable
                        .Where(x => appointment.AppointmentInformation.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                        .OrderBy(x => x.MilitaryTime).ToList();
                    var item = new AppointmentTable
                    {
                        Id = appointment.AppointmentInformation.Id,
                        Product = await _context.Products.FindAsync(appointment.AppointmentInformation.ProductId),
                        PatientInformation = appointment.PatientInformation,
                        PaymentMethod = appointment.AppointmentInformation.PaymentMethod,
                        AspNetUserId = appointment.AppointmentInformation.AspNetUserId,
                        AppointmentTime = selectedSlots,
                        AppointmentTimeLabel = selectedSlots.Count > 1
                            ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[selectedSlots.Count - 1].Name.Split("-")[0].Trim()}"
                            : selectedSlots[0].Name,
                        AppointmentDate = appointment.AppointmentInformation.AppointmentDate,
                        IsPaid = appointment.AppointmentInformation.IsPaid,
                        Status = appointment.AppointmentInformation.Status.GetDisplayName(),
                        IsWalkIn = appointment.AppointmentInformation.IsWalkIn,
                        TransactionId = appointment.AppointmentInformation.TransactionId,
                        ReferenceNumber = appointment.AppointmentInformation.ReferenceNumber,
                        IsPwd = appointment.AppointmentInformation.IsPwd,
                        IsPregnant = appointment.AppointmentInformation.IsPregnant,
                        IsSenior = appointment.AppointmentInformation.IsSenior
                    };
                    
                    if (!string.IsNullOrEmpty(appointment.AppointmentInformation.MedCert) && System.IO.File.Exists(appointment.AppointmentInformation.MedCert))
                    {
                        byte[] imageArray = System.IO.File.ReadAllBytes(appointment.AppointmentInformation.MedCert);
                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                        item.MedCert = base64ImageRepresentation;
                    }
                    else
                    {
                        item.MedCert = string.Empty;
                    }
                    appointmentList.Add(item);
                }
            }
            return Ok(new { data = appointmentList, totalRecords = totalRecords });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetAppointments/{id}")]
    public async Task<IActionResult> GetAppointment(int id)
    {
        try
        {
            var appointment = await _context.AppointmentInformations
                .FirstOrDefaultAsync(x => x.Id == id);
            var isAdmin = User.IsInRole("Admin");
            if(!isAdmin)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (appointment != null && appointment.AspNetUserId != userId)
                {
                    return BadRequest("Appointment Details not found!");
                } 
            }
            
            return Ok(appointment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetPatientInformation")]
    public async Task<IActionResult> GetPatientInformation()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var patientInformation = _context.PatientInformations.Where(x => x.UserId == userId);
        return Ok(patientInformation);
    }

    [HttpPost]
    [Route("AddAppointment")]
    public async Task<IActionResult> AddAppointment([FromForm] AddAppointmentPayload appointment)
    {
        var appointmentInformation = new AppointmentInformation();
        try
        {
            if (ModelState.IsValid)
            {
                var referenceNumber = Guid.NewGuid().ToString();
                var timeslots = await _context.AppointmentTimes.ToListAsync();
                var product = await _context.Products.FindAsync(appointment.AppointmentDetails.Schedule.Product);
                var currentTimeslot =
                    timeslots.FirstOrDefault(x => x.Id == appointment.AppointmentDetails.Schedule.AppointmentTime);
                var selectedTimeslots = timeslots.Where(x => x.MilitaryTime >= currentTimeslot.MilitaryTime && x.MilitaryTime < currentTimeslot.MilitaryTime + product.Duration);
                var patientInformation = _mapper.Map<PatientInformation>(appointment.AppointmentDetails.PersonalInformation);
                var userId = string.Empty;
                var medCert = "";
                
                if (!appointment.IsWalkIn)
                {
                    userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    patientInformation = await _context.PatientInformations.FirstOrDefaultAsync(x => x.UserId == userId);
                }
                else
                {
                    await _context.PatientInformations.AddAsync(patientInformation);
                    await _context.SaveChangesAsync();
                }
                
                if (appointment.AppointmentDetails.PersonalInformation.MedCert != null)
                {
                    string uploads =
                        Path.Combine(
                            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                            "MedCerts");
                    var extension = Path.GetExtension(appointment.AppointmentDetails.PersonalInformation.MedCert.FileName);
                    var filename = Guid.NewGuid().ToString();
                    string filePath = Path.Combine(uploads, $"{filename}{extension}");
                    CheckFolderExists(uploads);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await appointment.AppointmentDetails.PersonalInformation.MedCert .CopyToAsync(fileStream);
                    }
                
                    medCert = filePath;
                }
                
                appointmentInformation = new AppointmentInformation
                {
                    ReferenceNumber = referenceNumber,
                    AppointmentTimeIds = String.Join(",", selectedTimeslots.Select(x => x.Id)),
                    Status = AppointmentStatusEnum.Pending,
                    AppointmentDate = appointment.AppointmentDetails.Schedule.AppointmentDate,
                    AspNetUserId = appointment.IsWalkIn ? null: userId,
                    IsPaid = appointment.PaymentType == PaymentMethodEnum.Paypal || appointment.IsPaid,
                    PaymentMethod = appointment.PaymentType,
                    PatientInformationId = patientInformation.Id,
                    ProductId = appointment.AppointmentDetails.Schedule.Product,
                    TransactionId = appointment.TransactionId,
                    AppointmentDuration = product.Duration,
                    MedCert = medCert,
                    Amount = product.Price,
                    IsPwd = appointment.AppointmentDetails.PersonalInformation.IsPwd,
                    IsSenior = appointment.AppointmentDetails.PersonalInformation.IsSenior,
                    IsPregnant = appointment.AppointmentDetails.PersonalInformation.IsPregnant
                };

                await _context.AppointmentInformations.AddAsync(appointmentInformation);
                await _context.SaveChangesAsync();
                var timeIds = appointmentInformation.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
                var selectedSlots = timeslots
                    .Where(x => appointmentInformation.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                    .OrderBy(x => x.MilitaryTime).ToList();
                var appointmentLabel = selectedSlots.Count > 1
                    ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[1].Name.Split("-")[0].Trim()}"
                    : selectedSlots[0].Name;

                if (patientInformation.Email != null && patientInformation.Email != string.Empty)
                {
                    if (appointmentInformation.PaymentMethod == PaymentMethodEnum.Paypal)
                    {
                        string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Receipt.html");
                        var template = System.IO.File.ReadAllText(path);
                        template = template.Replace("{{name}}", $"{patientInformation.FirstName} {patientInformation.LastName}");
                        template = template.Replace("{{appointment_date}}", $"{appointmentInformation.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}");
                        template = template.Replace("{{receipt_id}}", appointmentInformation.TransactionId);
                        template = template.Replace("{{date}}", DateTime.Now.ConvertTime().ToString("MM/dd/yyyy"));
                        template = template.Replace("{{description}}", product.Name);
                        template = template.Replace("{{amount}}", $"Php {product.Price.ToString("N2")}");
                        template = template.Replace("{{total}}", $"Php {product.Price.ToString("N2")}");
                        SendEmail(patientInformation.Email, template, "Schedule Confirmation");
                    }
                    else
                    {
                        var emailBody =
                            $"Appointment on {appointmentInformation.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel} has been scheduled to you.<br/>" +
                            $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise your appointment will be reschedule." +
                            $"\n\rPlease don't forget to settle your payment at the clinic.";
                        SendEmail(patientInformation.Email, emailBody, "Schedule Confirmation");
                    }
                }

                if (patientInformation.Phone != null && patientInformation.Phone != string.Empty)
                {
                    var smsMessage = $"Your appointment on {appointmentInformation.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel} " +
                        $"has been scheduled. Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise your appointment will be reschedule.\r\n ";
                    SendSms(patientInformation.Phone, smsMessage);
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = StatusEnum.Error.GetDisplayName(), 
                Message = "Error Saving Appointment" + "/n/n" + ex.Message + "/n/n" + ex.StackTrace });
        }
        return StatusCode(StatusCodes.Status200OK, new Response { Status = StatusEnum.Success.GetDisplayName(), Message = "Successfully added Appointment" });
    }

    [HttpGet]
    [Route("GetServiceCatalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServiceCatalog()
    {
        var products = await _context.Products.Where(x => x.IsActive).ToListAsync();
        foreach (var product in products)
        {
            if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(product.Image))
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(product.Image);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                product.Image = base64ImageRepresentation;
            }
        }
        return Ok(products);
    }

    [HttpPost]
    [Route("Reschedule")]
    public async Task<IActionResult> Reschedule(ReschedulePayload model)
    {
        try
        {
            var appointment = await _context.AppointmentInformations.FindAsync(model.AppointmentId);
            appointment.AppointmentDate = model.AppointmentDate;
            appointment.Status = AppointmentStatusEnum.Pending;
            
            var timeslots = await _context.AppointmentTimes.ToListAsync();
            var product = await _context.Products.FindAsync(model.ProductId);
            var currentTimeslot =
                timeslots.FirstOrDefault(x => x.Id == model.AppointmentTime);
            var selectedTimeslots = timeslots.Where(x => x.MilitaryTime >= currentTimeslot.MilitaryTime && x.MilitaryTime < currentTimeslot.MilitaryTime + product.Duration);
            appointment.AppointmentTimeIds = String.Join(",", selectedTimeslots.Select(x => x.Id));
            _context.AppointmentInformations.Update(appointment);
            await _context.SaveChangesAsync();

            var patientInformation = await _context.PatientInformations.FirstOrDefaultAsync(x => x.Id == appointment.Id);

            var timeIds = appointment.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
            var selectedSlots = timeslots
                .Where(x => appointment.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                .OrderBy(x => x.MilitaryTime).ToList();
            var appointmentLabel = selectedSlots.Count > 1
                ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[1].Name.Split("-")[0].Trim()}"
                : selectedSlots[0].Name;
            if (patientInformation.Email != null && patientInformation.Email != string.Empty)
            {
                if (appointment.PaymentMethod == PaymentMethodEnum.Paypal)
                {
                    var emailBody =
                        $"Your appointment  has been rescheduled on {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}.<br/>" +
                        $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise wise your appointment will be reschedule.";
                    SendEmail(patientInformation.Email, emailBody, "Reschedule Confirmation");
                }
                else
                {
                    var emailBody =
                        $"Appointment on {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel} has been scheduled to you.<br/>" +
                        $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise wise your appointment will be reschedule." +
                        $"\n\rPlease don't forget to settle your payment at the clinic.";
                    SendEmail(patientInformation.Email, emailBody, "Schedule Confirmation");
                }
            }

            if (patientInformation.Phone != null && patientInformation.Phone != string.Empty)
            {
                var smsMessage = $"Your appointment  has been rescheduled on {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}.<br/>" +
                        $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise wise your appointment will be reschedule.";
                SendSms(patientInformation.Phone, smsMessage);
            }

            return StatusCode(StatusCodes.Status200OK, new Response { Status = StatusEnum.Success.GetDisplayName(), Message = "Successfully Rescheduled Appointment" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = StatusEnum.Error.GetDisplayName(), 
                Message = "Error Saving Appointment" + "/n/n" + ex.Message + "/n/n" + ex.StackTrace });
        }
    }

    [HttpGet]
    [Route("GetAppointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var isAdmin = User.IsInRole("Admin");

        var info = await _context.AppointmentInformations
            .Join(_context.PatientInformations, x => x.PatientInformationId, y => y.Id,
                (x, y) => new { PatientInformation = y, AppointmentInformation = x })
            .Where(x => x.AppointmentInformation.IsActive)
            .ToListAsync();
        if (!isAdmin)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            info = info.Where(x => x.PatientInformation.UserId == userId).ToList();
        }
        
        return Ok(info);
    }


    [HttpGet]
    [Route("GetImage/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImage(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        
        if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(product.Image))
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(product.Image);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            return Ok(base64ImageRepresentation);
        }

        return Ok(string.Empty);
    }

    private void CheckFolderExists(string filePath)
    {
        bool exists = System.IO.Directory.Exists(filePath);
        if(!exists)
            System.IO.Directory.CreateDirectory(filePath);
    }
}
using System.Data.OleDb;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using htdc_api.Authentication;
using htdc_api.Enumerations;
using htdc_api.Interface;
using htdc_api.Models;
using htdc_api.Models.Authentication;
using htdc_api.Models.Payloads;
using htdc_api.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi.Extensions;

namespace htdc_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AdminController : BaseController
{
    public AdminController(
        ApplicationDbContext context,
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        IConfiguration configuration) :
        base(context, mapper, userManager, emailSender, configuration)
    {
    }

    [HttpGet]
    [Route("GetServices")]
    public async Task<IActionResult> GetServices()
    {
        var returnModel = new List<ServiceAppointmentViewModel>();
        try
        {
            var isAdmin = User.IsInRole("Admin");
            List<Products>? products;
            if (isAdmin)
            {
                products = await _context.Products.Where(x => x.IsActive).ToListAsync();
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.AspNetUserId == userId);
                if (profile == null)
                {
                    return NotFound();
                }

                products = await _context.Products.Where(x => x.IsActive).ToListAsync();
                if (profile.IsPwd)
                {
                    products = products.Where(x => x.AllowPwd).ToList();
                }

                if (profile.IsPregnant)
                {
                    products = products.Where(x => x.AllowPregnant).ToList();
                }

                if (profile.IsSenior)
                {
                    products = products.Where(x => x.AllowSenior).ToList();
                }
            }

            foreach (var product in products)
            {
                //if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(product.Image))
                //{
                //    byte[] imageArray = System.IO.File.ReadAllBytes(product.Image);
                //    string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                //    product.Image = base64ImageRepresentation;
                //}
                //else
                //{
                //    product.Image = string.Empty;
                //}

                returnModel.Add(new ServiceAppointmentViewModel
                {
                    Name = product.Name,
                    Price = product.Price,
                    Id = product.Id
                });
            }

            return Ok(returnModel);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetServicesById/{id}")]
    public async Task<IActionResult> GetServicesById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(product.Image))
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(product.Image);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            product.Image = base64ImageRepresentation;
        }
        else
        {
            product.Image = string.Empty;
        }
        return Ok(product);
    }

    [HttpPost]
    [Route("CreateService")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateService([FromForm] CreateProductsViewModel model)
    {
        try
        {
            var product = _mapper.Map<Products>(model);
            if (model.Thumbnail != null)
            {
                string uploads =
                    Path.Combine(
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                        "ServicesThumbnails");
                var extension = Path.GetExtension(model.Thumbnail.FileName);
                var filename = Guid.NewGuid().ToString();
                string filePath = Path.Combine(uploads, $"{filename}{extension}");
                CheckFolderExists(uploads);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Thumbnail.CopyToAsync(fileStream);
                }
                
                product.Image = filePath;
                product.ImageFileName = model.Thumbnail.FileName;
            }
            
            var products = await _context.Products
                .Where(x => x.Name.ToLower() == product.Name.ToLower() || x.Code == product.Code).ToListAsync();
            if (products.Any())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Product Already Exist!" });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return Ok(new Response
                { Status = StatusEnum.Success.GetDisplayName(), Message = "Product created successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost]
    [Route("UpdateService")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateService([FromForm] CreateProductsViewModel model)
    {
        try
        {
            var existingProduct = await _context.Products.Where(x => (x.Name == model.Name || x.Code == model.Code) && x.Id != model.Id)
                .ToListAsync();
            if (existingProduct.Any())
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Product Code/Name already exist!" });
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (product == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Product Does not Exist!" });
            }
            
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Code = model.Code;
            product.Duration = model.Duration;
            
            if (model.Thumbnail != null)
            {
                string uploads =
                    Path.Combine(
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                        "ServicesThumbnails");
                var extension = Path.GetExtension(model.Thumbnail.FileName);
                var filename = Guid.NewGuid().ToString();
                string filePath = Path.Combine(uploads, $"{filename}{extension}");
                CheckFolderExists(uploads);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Thumbnail.CopyToAsync(fileStream);
                }
                
                product.Image = filePath;
                product.ImageFileName = model.Thumbnail.FileName;
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(new Response
                { Status = StatusEnum.Success.GetDisplayName(), Message = "Product updated successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    
    [HttpDelete]
    [Route("DeleteProduct/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Item Doesn't Exist!" });
            }

            product.IsActive = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(new Response
                { Status = StatusEnum.Success.GetDisplayName(), Message = "Product updated successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost]
    [Route("CreateAppointment")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAppointment([FromForm] CreateAppointment payload)
    {
        try
        {
            var patientInformation = new PatientInformation();
            var appointmentInformation = new AppointmentInformation();
            appointmentInformation.PatientInformationId = patientInformation.Id;
            var referenceNumber = Guid.NewGuid().ToString();
            var timeslots = await _context.AppointmentTimes.ToListAsync();
            var product = await _context.Products.FindAsync(payload.Schedule.Product);
            var currentTimeslot =
                timeslots.FirstOrDefault(x => x.Id == payload.Schedule.AppointmentTime);
            var selectedTimeslots = timeslots.Where(x =>
                x.MilitaryTime >= currentTimeslot.MilitaryTime &&
                x.MilitaryTime < currentTimeslot.MilitaryTime + product.Duration);
            var medCert = "";

            if (payload.PatientInformation.MedCert != null)
            {
                string uploads =
                    Path.Combine(
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                        "MedCerts");
                var extension = Path.GetExtension(payload.PatientInformation.MedCert.FileName);
                var filename = Guid.NewGuid().ToString();
                string filePath = Path.Combine(uploads, $"{filename}{extension}");
                CheckFolderExists(uploads);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await payload.PatientInformation.MedCert .CopyToAsync(fileStream);
                }
                
                medCert = filePath;
            }
            
            if (payload.PatientInformation.Id > 0)
            {
                patientInformation =
                    await _context.PatientInformations.FirstOrDefaultAsync(x => x.Id == payload.PatientInformation.Id);
                patientInformation.FirstName = payload.PatientInformation.FirstName;
                patientInformation.LastName = payload.PatientInformation.LastName;
                patientInformation.Address = payload.PatientInformation.Address;
                patientInformation.Phone = payload.PatientInformation.Phone;
                patientInformation.Email = payload.PatientInformation.Email;
                patientInformation.IsPregnant = payload.PatientInformation.IsPregnant;
                patientInformation.IsSenior = payload.PatientInformation.IsSenior;
                patientInformation.IsPwd = payload.PatientInformation.IsPwd;
                patientInformation.DateUpdated = DateTime.UtcNow;
                _context.PatientInformations.Update(patientInformation);
                await _context.SaveChangesAsync();
            }
            else
            {
                patientInformation = _mapper.Map<PatientInformation>(payload.PatientInformation);
                await _context.PatientInformations.AddAsync(patientInformation);
                await _context.SaveChangesAsync();
            }

            appointmentInformation = new AppointmentInformation
            {
                ReferenceNumber = referenceNumber,
                AppointmentTimeIds = String.Join(",", selectedTimeslots.Select(x => x.Id)),
                Status = AppointmentStatusEnum.Pending,
                AppointmentDate = payload.Schedule.AppointmentDate,
                AspNetUserId = patientInformation.UserId,
                PaymentMethod = PaymentMethodEnum.Cash,
                PatientInformationId = patientInformation.Id,
                ProductId = payload.Schedule.Product,
                TransactionId = "",
                IsWalkIn = payload.Schedule.IsWalkIn,
                AppointmentDuration = product.Duration,
                MedCert = medCert,
                Amount = product.Price,
                IsPwd = payload.PatientInformation.IsPwd,
                IsSenior = payload.PatientInformation.IsSenior,
                IsPregnant = payload.PatientInformation.IsPregnant
            };

            await _context.AppointmentInformations.AddAsync(appointmentInformation);
            await _context.SaveChangesAsync();
            
            var timeIds = appointmentInformation.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
            var selectedSlots = timeslots
                .Where(x => appointmentInformation.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                .OrderBy(x => x.MilitaryTime).ToList();
            var appointmentLabel = selectedSlots.Count > 1
                ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[selectedSlots.Count - 1].Name.Split("-")[0].Trim()}"
                : selectedSlots[0].Name;

            var emailBody = $"Hi {patientInformation.FirstName} {patientInformation.LastName}!<br/><br/>" +
            $"Your appointment is scheduled<br/>" +
            $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise your appointment will be reschedule.<br/><br/>" +
            $"NAME: {patientInformation.FirstName} {patientInformation.LastName}<br/>" + 
            $"Phone: {patientInformation.Phone}<br/>" +
            $"Email: {patientInformation.Email}<br/>" +
            $"Appointment Details: {appointmentInformation.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}<br/>" +
            $"Service: {product.Name}<br/><br/>" + 
            $"Thanks, <br/>Ocampo Dental Clinic<br/><br/>";

            if (patientInformation.Email != null && patientInformation.Email != string.Empty)
            {
                SendEmail(patientInformation.Email, emailBody, "Schedule Confirmation");                
            }

            if (patientInformation.Phone != null && patientInformation.Phone != string.Empty)
            {
                emailBody = $"Hi {patientInformation.FirstName} {patientInformation.LastName}!\r\n\r\n" +
                    $"Your appointment is scheduled\r\n" +
                    $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise your appointment will be reschedule.\r\n\r\n" +
                    $"Phone: 09971563948\r\n" +
                    $"Email: joashpatawaran@gmail.com\r\n" +
                    $"Appointment Details: {appointmentInformation.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}\r\n" +
                    $"Service: {product.Name}\r\n\r\n" +
                    $"Thanks, \r\nOcampo Dental Clinic\r\n\r\n" +
                    $"NAME: Joash Patawaran\r\n";
                SendSms(patientInformation.Phone, emailBody);
            }

            return StatusCode(StatusCodes.Status200OK,
                new Response
                    { Status = StatusEnum.Success.GetDisplayName(), Message = "Successfully added Appointment" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new Response
            {
                Status = StatusEnum.Error.GetDisplayName(),
                Message = "Error Saving Appointment" + "/n/n" + ex.Message + "/n/n" + ex.StackTrace
            });
        }
    }

    [HttpGet]
    [Route("GetPatients")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPatients()
    {
        try
        {
            var returnModel = new List<PatientInformation>();
            var patientList = await _context.PatientInformations
                .Where(x => x.IsActive).ToListAsync();
            var users = _userManager.Users;
            var patientRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == UserRoles.Patient);
            var userRoles = await _context.UserRoles.ToListAsync();
            foreach (var patient in patientList)
            {
                if (!string.IsNullOrEmpty(patient.UserId))
                {
                    var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.AspNetUserId == patient.UserId);
                    var role = userRoles.FirstOrDefault(x => x.UserId == patient.UserId);
                    if (role != null && profile.IsActive)
                    {
                        if (role.RoleId == patientRole.Id)
                        {
                            returnModel.Add(patient);
                        }

                    }
                }
                else
                {
                    returnModel.Add(patient);
                }
            }

            return Ok(returnModel);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message + "/n/n" + ex.StackTrace);
        }
    }

    [HttpGet]
    [Route("GetAllUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers(int page, int rows, string? filter)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userList = new List<UsersViewModel>();
        var totalRows = _userManager.Users.Join(_context.UserProfiles, 
            user => user.Id, 
            profile => profile.AspNetUserId,
            ((user, profile) => new { User = user, Profile = profile})).Count(x => x.Profile.IsActive && x.User.Id != currentUser&& (filter == null || x.User.Email.Contains(filter)));
        var users = _userManager.Users
            .Where(x => x.Id != currentUser && (filter == null || x.Email.Contains(filter)))
            .Skip((page - 1) * rows)
            .Take(rows).ToList();
        foreach (var user in users)
        {
            var details = new UsersViewModel
            {
                Email = user.Email,
                UserId = user.Id,
                UserName = user.UserName
            };

            var profile = _context.UserProfiles.FirstOrDefault(x => x.AspNetUserId == user.Id && x.IsActive);
            var patientInformation = await _context.PatientInformations.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (profile != null)
            {
                details.Avatar = profile.Avatar;
                details.FirstName = profile.FirstName;
                details.LastName = profile.LastName;
                details.DateCreated = profile.DateCreated;
                details.PatientInformation = patientInformation;
                userList.Add(details);
            }

        }

        return Ok(new { data = userList , totalRecords = totalRows });
    }

    [HttpGet]
    [Route("GetRoles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoles()
    {
        var list = await _context.Roles.ToListAsync();
        var roles = new List<DropDownViewModel>();
        foreach (var role in list)
        {
            roles.Add(new DropDownViewModel
            {
                Code = role.Id,
                Name = role.Name
            });
        }

        return Ok(roles);
    }


    [HttpGet]
    [Route("GetUserById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Ok("Record Not Found");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var details = new UsersViewModel
        {
            Email = user.Email,
            UserId = user.Id,
            UserName = user.UserName,
            Role = userRoles[0]
        };

        var profile = _context.UserProfiles.FirstOrDefault(x => x.AspNetUserId == user.Id);
        if (profile != null)
        {
            details.Avatar = profile.Avatar;
            details.FirstName = profile.FirstName;
            details.LastName = profile.LastName;
            details.Phone = profile.Phone;
            details.IsPwd = profile.IsPwd;
            details.IsPregnant = profile.IsPregnant;
            details.IsSenior = profile.IsSenior;
            details.UserId = id;
            details.PatientInformation = _context.PatientInformations.FirstOrDefault(x => x.UserId == user.Id);
        }

        return Ok(details);
    }

    [HttpPost]
    [Route("CreateUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] UsersViewModel model)
    {
        using (var identitydbContextTransaction = _context.Database.BeginTransaction())
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var userExists = await _userManager.FindByEmailAsync(model.Email);
                    if (userExists != null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response
                                { Status = StatusEnum.Error.GetDisplayName(), Message = "Email already exists!" });
                    }

                    IdentityUser user = new()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response
                            {
                                Status = StatusEnum.Error.GetDisplayName(),
                                Message = "SignUp failed! Please check details and try again."
                            });
                    }

                    await _userManager.AddToRoleAsync(user, model.Role);

                    var profile = new UserProfile
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        AspNetUserId = user.Id,
                        Avatar = "",
                        IsActive = true,
                        IsPwd = model.IsPwd,
                        IsSenior = model.IsSenior,
                        IsPregnant = model.IsPregnant,
                        Phone = model.Phone
                    };

                    var patientInformation = _mapper.Map<PatientInformation>(profile);
                    patientInformation.Email = model.Email;
                    profile.PatientInformation = patientInformation;
                    await _context.UserProfiles.AddAsync(profile);
                }

                await _context.SaveChangesAsync();
                identitydbContextTransaction.Commit();

                return Ok(new Response
                    { Status = StatusEnum.Success.GetDisplayName(), Message = "Sign up successfully!" });
            }
            catch (Exception ex)
            {
                identitydbContextTransaction.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = StatusEnum.Error.GetDisplayName(), Message = ex.Message + "/n/n/n" + ex.StackTrace
                    });
            }
        }
    }

    [HttpPost]
    [Route("UpdateUser")]
    public async Task<IActionResult> Update([FromBody] UsersViewModel model)
    {
        try
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                if (currentUser != model.UserId)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Status = StatusEnum.Error.GetDisplayName(),
                            Message = "Invalid User!"
                        });
                }
            }
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByIdAsync(model.UserId);
                if (userExists == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Status = StatusEnum.Error.GetDisplayName(),
                            Message = "User update failed! Please check user details and try again."
                        });
                }

                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(userExists);
                    var result = await _userManager.ResetPasswordAsync(userExists, token, model.Password);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status406NotAcceptable,
                            new Response
                            {
                                Status = StatusEnum.Error.GetDisplayName(), Message = result.Errors.First().Description
                            });
                    }
                }

                //remove roles first
                var roles = await _userManager.GetRolesAsync(userExists);
                await _userManager.RemoveFromRolesAsync(userExists, roles.ToArray());

                //then add new role
                await _userManager.AddToRoleAsync(userExists, model.Role);

                var userProfile = _context.UserProfiles.First(x => x.AspNetUserId == model.UserId);
                var patientInformation = await _context.PatientInformations.FirstOrDefaultAsync(x => x.UserId == model.UserId);
                
                userProfile.FirstName = model.FirstName;
                userProfile.LastName = model.LastName;
                userProfile.AspNetUserId = model.UserId;
                userProfile.Avatar = "";
                userProfile.IsPwd = model.IsPwd;
                userProfile.IsPregnant = model.IsPregnant;
                userProfile.IsSenior = model.IsSenior;
                userProfile.Phone = model.Phone;
                
                patientInformation.FirstName = model.FirstName;
                patientInformation.LastName = model.LastName;
                patientInformation.Email = model.Email;
                patientInformation.Phone = model.Phone;
                patientInformation.IsSenior = model.IsSenior;
                patientInformation.IsPregnant = model.IsPregnant;
                patientInformation.IsPwd = model.IsPwd;
                patientInformation.Address = model.Address;

                _context.UserProfiles.Update(userProfile);
                _context.PatientInformations.Update(patientInformation);
                
                _context.SaveChanges();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = StatusEnum.Error.GetDisplayName(), Message = "Error Saving" });
            }

            return Ok(new Response
                { Status = StatusEnum.Success.GetDisplayName(), Message = "User Updated successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpDelete]
    [Route("DeleteUser/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var user = await _context.UserProfiles.FirstOrDefaultAsync(x => x.AspNetUserId == id);
            if (user != null)
            {
                var patientInfo =
                    await _context.PatientInformations.FirstOrDefaultAsync(x => x.UserId == user.AspNetUserId);
                if (patientInfo != null)
                {
                    patientInfo.IsActive = false;
                    _context.PatientInformations.Update(patientInfo);
                }
                
                user.IsActive = false;
                _context.UserProfiles.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new Response
                    { Status = StatusEnum.Success.GetDisplayName(), Message = "User deleted successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = StatusEnum.Error.GetDisplayName(), Message = "User not found!"
                    });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response
                {
                    Status = StatusEnum.Error.GetDisplayName(), Message = ex.Message + "/n/n/n" + ex.StackTrace
                });
        }
    }

    [HttpDelete]
    [Route("DeleteAppointment/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        try
        {
            var appointment = await _context.AppointmentInformations.FindAsync(id);
            appointment.Status = AppointmentStatusEnum.Cancelled;
            _context.AppointmentInformations.Update(appointment);
            await _context.SaveChangesAsync();
            var patientInfo = await _context.PatientInformations.FindAsync(appointment.PatientInformationId);
            var timeslots = await _context.AppointmentTimes.ToListAsync();
            var timeIds = appointment.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
            var selectedSlots = timeslots
                .Where(x => appointment.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                .OrderBy(x => x.MilitaryTime).ToList();
            var appointmentLabel = selectedSlots.Count > 1
                ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[selectedSlots.Count - 1].Name.Split("-")[0].Trim()}"
                : selectedSlots[0].Name;
            
            if (patientInfo != null && !string.IsNullOrEmpty(patientInfo.Email))
            {
                if (appointment.PaymentMethod == PaymentMethodEnum.Paypal)
                {
                    SendEmail(patientInfo.Email, $"Appointment on {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel} has been cancelled and refund has been processed to your paypal account.", "Refund");
                }
                else if(appointment.PaymentMethod == PaymentMethodEnum.Cash)
                {
                    SendEmail(patientInfo.Email, $"Appointment on {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel} has been cancelled.", "Cancelled Appointment");
                }
            }
            
            return Ok(new Response
                { Status = StatusEnum.Success.GetDisplayName(), Message = "Success!" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response
                {
                    Status = StatusEnum.Error.GetDisplayName(), Message = ex.Message + "/n/n/n" + ex.StackTrace
                });
        }
    }

    [HttpPost]
    [Route("MarkAppointmentDone/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAppointmentDone(int id, [FromForm] CompleteAppointmentPayload model)
    {
        var appointment = await _context.AppointmentInformations.FindAsync(id);
        appointment.Status = AppointmentStatusEnum.Done;
        appointment.Prescriptions = model.Prescription;
        appointment.IsPaid = true;
        var files = HttpContext.Request.Form.Files;
        if (files != null && files.Count > 0)
        {
            var attachments = new List<AppointmentAttachments>();
            string uploads =
                Path.Combine(
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                    "AppointmentAttachments");
            CheckFolderExists(uploads);
            foreach (var file in files)
            {
                var filename = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploads, $"{filename}{extension}");
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                attachments.Add(new AppointmentAttachments
                {
                    FileName = file.FileName,
                    Location = filePath
                });
            }
            appointment.Attachments = attachments;
        }
        
        _context.AppointmentInformations.Update(appointment);
        await _context.SaveChangesAsync();
        return Ok(new Response
            { Status = StatusEnum.Success.GetDisplayName(), Message = "Success!" });
    }

    [HttpPost]
    [Route("GetSales")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSales([FromBody] SalesPayload model)
    {
        var info = await _context.AppointmentInformations
            .Join(_context.PatientInformations,
                information => information.PatientInformationId,
                patient => patient.Id,
                (information, patient) => new { information, patient }
            )
            .Join(_context.Products,
                x => x.information.ProductId,
                p => p.Id,
                (x, p) => new { information = x.information, productInfo = p, patientInfo = x.patient })
            .Where(x => x.information.IsActive 
                        && x.information.AppointmentDate >= model.DateFrom 
                        && x.information.AppointmentDate <= model.DateTo
            )
            .ToListAsync();

        if (model.Status != "All")
        {
            info = info.Where(x => x.information.Status.GetDisplayName() == model.Status).ToList();
        }

        var timeTable = await _context.AppointmentTimes.ToListAsync();

        var sales = new List<SalesViewModel>();

        foreach (var item in info)
        {
            var timeIds = item.information.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
            var selectedSlots = timeTable
                .Where(x => item.information.AppointmentTimeIds != null && timeIds.Contains(x.Id))
                .OrderBy(x => x.MilitaryTime).ToList();

            var timeLabel = selectedSlots.Count > 1
                ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[selectedSlots.Count - 1].Name.Split("-")[0].Trim()}"
                : selectedSlots[0].Name;

            sales.Add(new SalesViewModel
            {
                PatientName = $"{item.patientInfo.FirstName} {item.patientInfo.LastName}",
                Service = item.productInfo.Name,
                Price = item.information.Amount ?? 0,
                Status = item.information.Status.ToString(),
                Appointment = $"{item.information.AppointmentDate.ToString("dd/MM/yyyy")} {timeLabel}"
            });
        }

        return Ok(sales);
    }

    [HttpGet]
    [Route("GetProfile")]
    public async Task<IActionResult> GetProfile()
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUser);
        if (user == null)
        {
            return Ok("Record Not Found");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var details = new UsersViewModel
        {
            Email = user.Email,
            UserId = user.Id,
            UserName = user.UserName,
            Role = userRoles[0]
        };

        var profile = _context.UserProfiles.FirstOrDefault(x => x.AspNetUserId == user.Id);
        if (profile != null)
        {
            details.Avatar = profile.Avatar;
            details.FirstName = profile.FirstName;
            details.LastName = profile.LastName;
            details.Phone = profile.Phone;
            details.IsPwd = profile.IsPwd;
            details.IsPregnant = profile.IsPregnant;
            details.IsSenior = profile.IsSenior;
            details.UserId = currentUser;
            details.PatientInformation = _context.PatientInformations.FirstOrDefault(x => x.UserId == user.Id);
        }

        return Ok(details);
    }
    
    [HttpGet]
    [Route("GetPatientHistory")]
    public async Task<IActionResult> GetPatientHistory()
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var info = await _context.AppointmentInformations
            .Include(x => x.Attachments)
            .Join(_context.PatientInformations,
                information => information.PatientInformationId,
                patient => patient.Id,
                (information, patient) => new { information, patient }
            )
            .Join(_context.Products,
                x => x.information.ProductId,
                p => p.Id,
                (x, p) => new { information = x.information, productInfo = p, patientInfo = x.patient })
            .Where(x => x.information.IsActive 
            && x.information.Status == AppointmentStatusEnum.Done && x.patientInfo.UserId == currentUser
            )
            .ToListAsync();
        
        foreach (var appointment in info)
        {
            if (appointment.information.Attachments.Any())
            {
                foreach (var attachment in appointment.information.Attachments)
                {
                    if (System.IO.File.Exists(attachment.Location))
                    {
                        attachment.Location = ConvertImageToBase64(attachment.Location);
                    }
                    else
                    {
                        attachment.Location = "";
                    }
                }
            }

            if (string.IsNullOrEmpty(appointment.information.MedCert) &&
                System.IO.File.Exists(appointment.information.MedCert))
            {
                appointment.information.MedCert = ConvertImageToBase64(appointment.information.MedCert);
            }
            else
            {
                appointment.information.MedCert = "";
            }
        }
        return Ok(info);
    }

    [HttpGet]
    [Route("GetPatientRecords")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatientRecords(int page, int rows, string? filter)
    {
        var totalRows = await _context.PatientInformations
            .Where(patient => patient.IsActive && (filter == null || patient.FirstName.Contains(filter) ||
                                                   patient.LastName.Contains(filter) || patient.Email.Contains(filter)))
            .CountAsync();
        
        var list = await _context.PatientInformations
            .Where(patient => patient.IsActive && (filter == null || patient.FirstName.Contains(filter) ||
                                                   patient.LastName.Contains(filter) || patient.Email.Contains(filter)))
            .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToListAsync();
        return Ok(new { data = list, totalRows = totalRows });
    }

    [HttpGet]
    [Route("GetPatientRecordById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPatientRecordById(int id)
    {
        var appointmentList = await _context.AppointmentInformations
            .Include(x => x.Attachments)
            .Join(_context.Products,
                information => information.ProductId,
                products => products.Id,
                (information, product) => new { information, product })
            .Where(x => x.information.PatientInformationId == id).ToListAsync();
        foreach (var appointment in appointmentList)
        {
            if (appointment.information.Attachments.Any())
            {
                foreach (var attachment in appointment.information.Attachments)
                {
                    if (System.IO.File.Exists(attachment.Location))
                    {
                        attachment.Location = ConvertImageToBase64(attachment.Location);
                    }
                    else
                    {
                        attachment.Location = "";
                    }
                }
            }

            if (string.IsNullOrEmpty(appointment.information.MedCert) &&
                System.IO.File.Exists(appointment.information.MedCert))
            {
                appointment.information.MedCert = ConvertImageToBase64(appointment.information.MedCert);
            }
            else
            {
                appointment.information.MedCert = "";
            }
        }

        return Ok(appointmentList.OrderByDescending(x => x.information.AppointmentDate));
    }
    
    [HttpPost]
    [Route("ApproveAppointment/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveAppointment(int id)
    {
        var appointment = await _context.AppointmentInformations.FindAsync(id);
        appointment.Status = AppointmentStatusEnum.Approved;
        
        _context.AppointmentInformations.Update(appointment);
        await _context.SaveChangesAsync();
        
        var patientInformation = await _context.PatientInformations.FindAsync(appointment.PatientInformationId);
        var timeslots = await _context.AppointmentTimes.ToListAsync();
        var product = await _context.Products.FindAsync(appointment.ProductId);

        var timeIds = appointment.AppointmentTimeIds.Split(',').Select(int.Parse).ToList();
        var selectedSlots = timeslots
            .Where(x => appointment.AppointmentTimeIds != null && appointment.AppointmentTimeIds.Contains(x.Id.ToString()))
            .OrderBy(x => x.MilitaryTime).ToList();
        var appointmentLabel = selectedSlots.Count > 1
            ? $"{selectedSlots[0].Name.Split("-")[0].Trim()} - {selectedSlots[selectedSlots.Count - 1].Name.Split("-")[0].Trim()}"
            : selectedSlots[0].Name;
        
        var emailBody = $"Hi {patientInformation.FirstName} {patientInformation.LastName}!<br/><br/>" +
            $"Your appointment is confirmed<br/><br/>" +
            $"NAME: {patientInformation.FirstName} {patientInformation.LastName}<br/>" +
            $"Phone: {patientInformation.Phone}<br/>" +
            $"Email: {patientInformation.Email}<br/>" +
            $"Appointment Details: {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}<br/>" +
            $"Service: {product.Name}<br/><br/>" +
            $"Thanks, <br/>Ocampo Dental Clinic<br/><br/>";
        
        if (patientInformation.Email != null && patientInformation.Email != string.Empty)
        {

            SendEmail(patientInformation.Email, emailBody, "Schedule Confirmation");
        }

        if (patientInformation.Phone != null && patientInformation.Phone != string.Empty)
        {
            emailBody = $"Hi {patientInformation.FirstName} {patientInformation.LastName}!\r\n\r\n" +
            $"Your appointment is confirmed\r\n\r\nThanks, \r\nOcampo Dental Clinic\r\n\r\n" +
            $"NAME: Joash Patawaran\r\n" +
            $"Phone: 09971563948\r\n" +
            $"Email: {patientInformation.Email}\r\n" +
            $"Appointment Details: {appointment.AppointmentDate.ToString("MM/dd/yyyy")} {appointmentLabel}\r\n" +
            $"Service: {product.Name}";

            SendSms(patientInformation.Phone, emailBody);
        }

        return Ok(new Response
            { Status = StatusEnum.Success.GetDisplayName(), Message = "Success!" });
    }
    
    [HttpPost]
    [Route("UpdateAttachments/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAttachments(int id, [FromForm] CompleteAppointmentPayload model)
    {
        var appointment = await _context.AppointmentInformations.FindAsync(id);
        appointment.Prescriptions = model.Prescription;
        var files = HttpContext.Request.Form.Files;
        if (files != null && files.Count > 0)
        {
            var attachments = new List<AppointmentAttachments>();
            string uploads =
                Path.Combine(
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                    "AppointmentAttachments");
            CheckFolderExists(uploads);
            foreach (var file in files)
            {
                var filename = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploads, $"{filename}{extension}");
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                attachments.Add(new AppointmentAttachments
                {
                    FileName = file.FileName,
                    Location = filePath
                });
            }
            appointment.Attachments = attachments;
        }
        
        _context.AppointmentInformations.Update(appointment);
        await _context.SaveChangesAsync();
        return Ok(new Response
            { Status = StatusEnum.Success.GetDisplayName(), Message = "Success!" });
    }

    [HttpGet]
    [Route("GetServiceBreakdownChart")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetServiceBreakdownChart()
    {
        var services = await _context.AppointmentInformations
            .Join(_context.Products, 
            information => information.ProductId, product => product.Id,
            (information, product) => new { information, product})
            .Where(x => x.information.Status == AppointmentStatusEnum.Done)
            .GroupBy(x => x.product.Name)
            .Select(x => new { ProductName = x.Key, Count = x.Count() }).ToListAsync();
        var totalItems = services.Sum(x => x.Count);
        var returnModel = new List<ServiceBreakdownViewModel>();
        foreach (var item in services) {
            returnModel.Add(new ServiceBreakdownViewModel
            {
                ProductName = item.ProductName,
                Percentage = Math.Round(((decimal)item.Count / (decimal)totalItems) * 100, 2)
            });
        }
        return Ok(returnModel);
    }

    [HttpPost]
    [Route("GetPatientChart")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPatientChart([FromBody] SalesPayload payload)
    {
        var patientList = await _context.PatientInformations
            .Join(_context.AppointmentInformations, patientInfo => patientInfo.Id, appointmentInfo => appointmentInfo.PatientInformationId,
            (patientInfo, appointmentInfo) => new { patientInfo, appointmentInfo })
            .Where(x => x.appointmentInfo.AppointmentDate.Date >= payload.DateFrom.Value.Date 
            && x.appointmentInfo.AppointmentDate.Date <= payload.DateTo.Value.Date
            && x.appointmentInfo.Status == AppointmentStatusEnum.Done)
            .ToListAsync();
        var newPatient = patientList.Where(x => x.patientInfo.DateCreated.Date >= payload.DateFrom.Value.Date && x.patientInfo.DateCreated.Date <= payload.DateTo.Value.Date).Select(x => x.appointmentInfo).Count();
        var oldPatient = patientList.Where(x => x.patientInfo.DateCreated.Date < payload.DateFrom.Value.Date).Select(x => x.appointmentInfo).Count();
        var totalPatients = 0;
        var model = new List<PatientChartViewModel>();

        totalPatients = newPatient + oldPatient;

        model.Add(new PatientChartViewModel
        {
            PatientType = "New Patient",
            Count = newPatient,
            Percentage = newPatient > 0 ? Math.Round((decimal)newPatient / (decimal)totalPatients * 100, 2) : 0
        });

        model.Add(new PatientChartViewModel
            {
                PatientType = "Old Patient",
                Count = oldPatient,
                Percentage = oldPatient > 0 ? Math.Round((decimal)oldPatient / (decimal)totalPatients * 100, 2) : 0
            });

        return Ok(model);
    }

    [HttpGet]
    [Route("GetServiceRevenue")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetServiceRevenue()
    {
        var services = await _context.Products.ToListAsync();
        var model = new List<ServiceRevenueViewModel> ();

        foreach (var service in services)
        {
            var productName = service.Name;
            var appointmentInfos = await _context.AppointmentInformations.Where(x => x.ProductId == service.Id && x.Status == AppointmentStatusEnum.Done).ToListAsync();
            if (appointmentInfos.Any())
            {
                model.Add(new ServiceRevenueViewModel
                {
                    ProductName = productName,
                    UnitPrice = service.Price,
                    ServiceCount = appointmentInfos.Count,
                    Total = service.Price * appointmentInfos.Count
                });
            }
        }

        return Ok(model);
    }


    #region helper

    private string ConvertImageToBase64(string location)
    {
        byte[] imageArray = System.IO.File.ReadAllBytes(location);
        return Convert.ToBase64String(imageArray);
    }


    private void CheckFolderExists(string filePath)
    {
        bool exists = System.IO.Directory.Exists(filePath);
        if(!exists)
            System.IO.Directory.CreateDirectory(filePath);
    }

    #endregion
}
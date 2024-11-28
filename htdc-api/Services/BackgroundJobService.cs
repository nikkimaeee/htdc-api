using Hangfire;
using htdc_api.Authentication;
using htdc_api.Extensions;
using htdc_api.Interface;
using htdc_api.Models.ViewModel;
using htdc_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Twilio.TwiML.Messaging;
using Twilio.TwiML.Voice;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace htdc_api.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        public IConfiguration _config { get; set; }
        public readonly ApplicationDbContext _context;
        public readonly IEmailSender _emailSender;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public BackgroundJobService(IConfiguration config, ApplicationDbContext context, IEmailSender emailSender, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) {
            _config = config;
            _context = context;
            _emailSender = emailSender;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        public void SendReminder()
        {
            _recurringJobManager.AddOrUpdate("Send Reminder", () => Remind(), Cron.Hourly);
        }

        public async void Remind()
        {
            var dateToday = DateTime.UtcNow.ConvertTime();
            var appointments = await _context.AppointmentInformations
                .Where(x => x.AppointmentDate.Date == dateToday.Date).ToListAsync();

            foreach (var appointment in appointments)
            {
                var reminder = await _context.AutoReminders.FirstOrDefaultAsync(x => x.AppointmentId == appointment.Id);
                if (reminder == null)
                {
                    var patientInfo = await _context.PatientInformations.Where(x => x.Id == appointment.PatientInformationId).FirstOrDefaultAsync();
                    if(patientInfo.Email != null)
                    {
                        var emailBody =
                        $"You have an appointment today.<br/>" +
                                $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise your appointment will be reschedule." +
                                $"\n\rPlease don't forget to settle your payment at the clinic.";

                        var email = new Models.Message
                        (
                            new string[] { patientInfo.Email },
                            "Schedule Reminder",
                            emailBody
                        );
                        _emailSender.SendEmail(email);
                    }

                    if (patientInfo.Phone != null) {
                        var emailBody =
                        $"You have an appointment today.<br/>" +
                                $"Please be in the clinic 10 minutes before the appointment, and you have a grace period of 10 to 15 minutes otherwise your appointment will be reschedule." +
                                $"\n\rPlease don't forget to settle your payment at the clinic.";

                        var sid = Environment.GetEnvironmentVariable("TwilioSid") ?? _config.GetSection("TwilioSid").Value;
                        var token = Environment.GetEnvironmentVariable("TwilioAuthToken") ?? _config.GetSection("TwilioAuthToken").Value;
                        var twilioNumber = Environment.GetEnvironmentVariable("TwilioNumber") ?? _config.GetSection("TwilioNumber").Value;
                        var num = Regex.Replace(patientInfo.Phone, "[^0-9_]", "");
                        var sendTo = $"+63{num}";
                        TwilioClient.Init(sid,
                            token);
                        var messageOptions = new CreateMessageOptions(
                            new PhoneNumber(sendTo));
                        messageOptions.From = new PhoneNumber(twilioNumber);
                        messageOptions.Body = emailBody;
                        var sms = MessageResource.Create(messageOptions);
                    }

                    var model = new AutoReminder
                    {
                        AppointmentId = appointment.Id,
                        IsProcessed = true
                    };

                    await _context.AutoReminders.AddAsync(model);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}

using System.Text.RegularExpressions;
using AutoMapper;
using htdc_api.Authentication;
using htdc_api.Interface;
using htdc_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace htdc_api.Controllers;

public class BaseController : ControllerBase
{
    public readonly ApplicationDbContext _context;
    public readonly UserManager<IdentityUser> _userManager;
    public readonly IMapper _mapper;
    public readonly IEmailSender _emailSender;
    public readonly IConfiguration _configuration;

    public BaseController(ApplicationDbContext context, 
        IMapper mapper, 
        UserManager<IdentityUser> userManager, 
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;
    }
    
    [ApiExplorerSettings(IgnoreApi=true)]
    public void SendEmail(string email, string body, string subject)
    {
        var message = new Message(
            new string[] { email }, 
            subject, 
            body);
        _emailSender.SendEmail(message);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void SendSms(string number, string message)
    {
        try
        {
            var sid = Environment.GetEnvironmentVariable("TwilioSid") ?? _configuration.GetSection("TwilioSid").Value;
            var token = Environment.GetEnvironmentVariable("TwilioAuthToken") ?? _configuration.GetSection("TwilioAuthToken").Value;
            var twilioNumber = Environment.GetEnvironmentVariable("TwilioNumber") ?? _configuration.GetSection("TwilioNumber").Value;
            var num = Regex.Replace(number, "[^0-9_]", "");
            var sendTo = $"+63{num}";
            TwilioClient.Init(sid,
                token);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(sendTo));
            messageOptions.From = new PhoneNumber(twilioNumber);
            messageOptions.Body = message;
            var sms = MessageResource.Create(messageOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

}
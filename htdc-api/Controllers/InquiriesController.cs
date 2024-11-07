using AutoMapper;
using htdc_api.Authentication;
using htdc_api.Enumerations;
using htdc_api.Interface;
using htdc_api.Models;
using htdc_api.Models.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace htdc_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InquiriesController : BaseController
{
    public InquiriesController(ApplicationDbContext context, 
        IMapper mapper, 
        UserManager<IdentityUser> userManager, 
        IEmailSender emailSender, IConfiguration configuration) : 
        base(context, mapper, userManager, emailSender, configuration)
    {
    }

    [HttpPost]
    [Route("SendInquiry")]
    [AllowAnonymous]
    public async Task<IActionResult> SendInquiry(InquiryPayload model)
    {
        var inquiry = _mapper.Map<Inquiry>(model);
        inquiry.Status = InquiryStatusEnum.Pending;
        await _context.Inquiries.AddAsync(inquiry);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route("GetInquiries")]
    public async Task<IActionResult> GetInquiries(int page, int rows, string status, DateTime dateFrom, DateTime dateTo,
        string? filter)
    {
        var statusEnum = (InquiryStatusEnum)Enum.Parse(typeof(InquiryStatusEnum), status);
        var totalRows = await _context.Inquiries
            .Where(x => x.IsActive &&
                        (filter == null || x.Email.Contains(filter) || x.Name.Contains(filter) || x.Subject.Contains(filter))
                        && x.DateCreated.Date >= dateFrom.Date && x.DateCreated.Date <= dateTo.Date
                        && (status == "All" || x.Status == statusEnum))
                        .CountAsync();

        var list = await _context.Inquiries
            .Where(x => x.IsActive &&
                        (filter == null || x.Email.Contains(filter) || x.Name.Contains(filter) ||
                         x.Subject.Contains(filter))
                        && x.DateCreated.Date >= dateFrom.Date && x.DateCreated.Date <= dateTo.Date
                        && (status == "All" || x.Status == statusEnum))
            .OrderByDescending(x => x.DateCreated)
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToListAsync();
        
        return Ok(new { data = list, totalRows });
    }

    [HttpGet]
    [Route("GetInquiriesById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetInquiriesById(int id)
    {
        var inquiry = await _context.Inquiries.FindAsync(id);
        return Ok(inquiry);
    }

    [HttpPost]
    [Route("CompleteInquiry")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CompleteInquiry([FromBody] CompleteInquiryPayload model)
    {
        var inquiry = await _context.Inquiries.FindAsync(model.Id);
        var subject = $"Re: {inquiry.Subject}";
        SendEmail(inquiry.Email, model.Message, subject);
        inquiry.Response = model.Message;
        inquiry.Status = InquiryStatusEnum.Resolved;
        _context.Inquiries.Update(inquiry);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
using htdc_api.Enumerations;

namespace htdc_api.Models;

public class Inquiry: BaseModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public string? Response { get; set; }
    public InquiryStatusEnum Status { get; set; }
}
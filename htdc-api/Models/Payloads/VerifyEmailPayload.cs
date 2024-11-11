namespace htdc_api.Models.Payloads;

public class VerifyEmailPayload
{
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
}
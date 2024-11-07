using System.ComponentModel.DataAnnotations;

namespace htdc_api.Models;

public class VerificationTokens
{
    [Key]
    public string Token { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiresDate { get; set; }
    public string AspNetUserId { get; set; }
}
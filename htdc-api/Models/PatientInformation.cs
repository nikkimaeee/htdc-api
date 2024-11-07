namespace htdc_api.Models;

public class PatientInformation: BaseModel
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsPwd  { get; set; }
    public bool IsSenior  { get; set; }
    public bool IsPregnant { get; set; }
}
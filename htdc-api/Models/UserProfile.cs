namespace htdc_api.Models;

public class UserProfile : BaseModel
{
    public UserProfile()
    {
        PatientInformation = new PatientInformation();
    }
    public string AspNetUserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Avatar { get; set; }

    public DateTime? LastLogin { get; set; }
    
    public bool IsPwd  { get; set; }
    
    public bool IsSenior  { get; set; }
    
    public bool IsPregnant { get; set; }
    
    public string? Phone { get; set; }
    
    public PatientInformation? PatientInformation { get; set; }

}
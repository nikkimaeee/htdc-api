namespace htdc_api.Models.ViewModel;

public class UsersViewModel
{
    public string? UserId { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Avatar { get; set; }

    public string Role { get; set; }
    
    public string Phone { get; set; }
    
    public PatientInformation? PatientInformation { get; set; }
    
    public string? Password { get; set; }
    
    public bool IsPwd  { get; set; }
    public bool IsSenior  { get; set; }
    public bool IsPregnant { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}
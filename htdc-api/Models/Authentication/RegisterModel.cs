namespace htdc_api.Models.Authentication;

public class RegisterModel
{

    public string Email { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Role { get; set; } = UserRoles.Patient;

    public string Phone { get; set; }
    
    public bool IsPwd { get; set; } 
    
    public bool IsSenior { get; set; } 
    
    public bool IsPregnant { get; set; } 
}
using htdc_api.Enumerations;

namespace htdc_api.Models.Payloads;

public class AddAppointmentPayload
{
    public AddAppointmentPayload()
    {
        AppointmentDetails = new AppointmentDetails();
    }
    
    public string? TransactionId { get; set; }
    public string Email { get; set; }
    public PaymentMethodEnum PaymentType { get; set; }
    public bool IsWalkIn { get; set; } = false;
    public bool IsPaid { get;set; } = false;
    public AppointmentDetails AppointmentDetails { get; set; }
}

public class Schedule
{
    public int Product { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int AppointmentTime { get; set; }
    public decimal Price { get; set; }
}

public class AppointmentDetails
{
    public Schedule Schedule { get; set; }
    public PatientInformationPayload PersonalInformation { get; set; }
}
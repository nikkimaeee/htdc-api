using htdc_api.Enumerations;

namespace htdc_api.Models.ViewModel;

public class AppointmentTable
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; }
    public string Status { get; set; }
    public DateTime AppointmentDate { get; set; }
    public List<AppointmentTime>? AppointmentTime { get; set; }
    public string AppointmentTimeLabel { get; set; }
    public Products? Product { get; set; }
    public string? AspNetUserId { get; set; }
    public string TransactionId { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public bool IsWalkIn { get; set; }
    public PatientInformation? PatientInformation { get; set; }
    public string MedCert { get; set; }
    public bool IsPwd { get; set; }
    public bool IsPregnant { get; set; }
    public bool IsSenior { get; set; }
}
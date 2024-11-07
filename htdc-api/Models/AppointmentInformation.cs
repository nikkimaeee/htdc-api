using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using htdc_api.Enumerations;

namespace htdc_api.Models;

public class AppointmentInformation: BaseModel
{
    public string ReferenceNumber { get; set; }
    public AppointmentStatusEnum Status { get; set; } = AppointmentStatusEnum.Pending;
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTimeIds { get; set; }
    public int ProductId { get; set; }
    public string? AspNetUserId { get; set; }
    public string? TransactionId { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public bool IsWalkIn { get; set; }
    public int? PatientInformationId { get; set; }
    public int AppointmentDuration { get; set; }
    public string? MedCert { get; set; }
    public decimal? Amount { get; set; }
    [MaxLength(10000)]
    public string? Prescriptions { get; set; }
    public List<AppointmentAttachments>? Attachments { get; set; }
}

public class AppointmentTimePayload
{
    public string AppointmentDate { get; set; }
    
    public int ServiceId { get; set; }
}
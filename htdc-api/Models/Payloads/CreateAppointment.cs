using htdc_api.Enumerations;

namespace htdc_api.Models.Payloads;

public class CreateAppointment
{
    public PatientInformationPayload PatientInformation { get; set; }
    public CreateSchedule Schedule { get; set; }
}

public class CreateSchedule: Schedule
{
    public PaymentMethodEnum PaymentType { get; set; }
    public bool IsWalkIn { get; set; }
}

public class PatientInformationPayload : PatientInformation
{
    public IFormFile? MedCert { get; set; }
}

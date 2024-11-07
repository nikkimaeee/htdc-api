namespace htdc_api.Models.Payloads;

public class CompleteAppointmentPayload
{
    public List<IFormFile>? Files { get; set; }
    public string? Prescription { get; set; }
}

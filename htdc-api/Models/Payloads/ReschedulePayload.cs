namespace htdc_api.Models.Payloads;

public class ReschedulePayload
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int AppointmentTime { get; set; }
    public int ProductId { get; set; }
}
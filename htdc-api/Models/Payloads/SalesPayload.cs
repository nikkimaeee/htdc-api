namespace htdc_api.Models.Payloads;

public class SalesPayload
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Status { get; set; }
}
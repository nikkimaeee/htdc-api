namespace htdc_api.Models;

public class AppointmentTime: BaseModel
{
    public string Name { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public int MilitaryTime { get; set; }
}
namespace htdc_api.Models
{
    public class AutoReminder
    {
        public int Id { get; set; }
        
        public int AppointmentId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; }

    }
}

namespace htdc_api.Models.ViewModel
{
    public class ServiceAppointmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Image { get; set; }

        public string ImageFileName { get; set; }

    }
}

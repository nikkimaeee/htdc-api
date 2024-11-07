namespace htdc_api.Models.ViewModel;

public class CreateProductsViewModel: BaseModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string Code { get; set; }
    public IFormFile? Thumbnail { get; set; }
}
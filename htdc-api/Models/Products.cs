namespace htdc_api.Models;

public class Products: BaseModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string Code { get; set; }
    public string Image { get; set; }
    
    public string ImageFileName { get; set; }
    public bool AllowPwd { get; set; } = true;
    public bool AllowSenior { get; set; } = true;
    public bool AllowPregnant { get; set; } = true;
}
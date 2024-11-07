using System.ComponentModel.DataAnnotations;

namespace htdc_api.Models;

public class BaseModel
{
    [Key]
    public int Id { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public DateTime? DateUpdated { get; set; }

    public bool IsActive { get; set; } = true;
}
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.RequestModels;

public class EmailModel
{
    [Required]
    public string Email { get; set; } = null!;
}

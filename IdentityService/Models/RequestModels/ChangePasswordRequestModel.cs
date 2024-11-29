using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.RequestModels;

public class ChangePasswordRequestModel
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string NewPassword { get; set; } = null!;
}

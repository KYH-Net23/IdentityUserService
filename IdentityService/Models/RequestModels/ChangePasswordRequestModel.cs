using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.RequestModels;

public class ChangePasswordRequestModel
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string NewPassword { get; set; } = null!;
}

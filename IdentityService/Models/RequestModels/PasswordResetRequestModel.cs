using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.RequestModels;

public class PasswordResetRequestModel
{
    [Required]
    public string ResetGuid { get; set; } = null!;
}

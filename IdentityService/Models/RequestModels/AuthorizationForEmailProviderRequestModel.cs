using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.RequestModels;

public class AuthorizationForEmailProviderRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;
}

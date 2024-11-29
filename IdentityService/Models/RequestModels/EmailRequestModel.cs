using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.RequestModels;

public class EmailRequestModel
{
    [Required]
    public string EmailAddress { get; init; } = null!;

    public string UserId { get; init; } = null!;
}

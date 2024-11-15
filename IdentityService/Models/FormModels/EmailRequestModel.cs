using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.FormModels;

public class EmailRequestModel
{
    [Required]
    public string Receiver { get; init; } = null!;

    public Uri? Uri { get; init; }
}

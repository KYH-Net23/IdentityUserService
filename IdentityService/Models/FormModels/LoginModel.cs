using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.FormModels;

public class LoginModel
{
	[StringLength(100, MinimumLength = 5)]
	public string Email { get; set; } = null!;
	[StringLength(100, MinimumLength = 5)]
	public string Password { get; set; } = null!;
}
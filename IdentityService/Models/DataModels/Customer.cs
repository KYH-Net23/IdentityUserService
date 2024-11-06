using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models.DataModels;

public class Customer : IdentityUser
{
	// Add extra

	[Required]
	[StringLength(
		200,
		MinimumLength = 5,
		ErrorMessage = "Username must be between 5 and 200 characters"
	)]
	public string StreetAddress { get; set; } = null!;

	public string? City { get; set; }

	public DateTime DateOfBirth { get; set; }

	public string? ProfilePictureUrl { get; set; }

	public DateTime AccountCreationDate { get; set; }

	public DateTime LastActiveDate { get; set; }

	public bool IsDeleted { get; set; }
}

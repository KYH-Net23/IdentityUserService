using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models.DataModels;

public class Customer : IdentityUser
{
	[Required]
	[StringLength(
		200,
		MinimumLength = 5,
		ErrorMessage = "Username must be between 5 and 200 characters"
	)]
	public string StreetAddress { get; set; } = null!;
	[Required]
	[StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
	public string City { get; set; } = null!;
	[Required]
	[StringLength(20, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 20 characters")]
	public string PostalCode { get; set; } = null!;
	[Required]
	public DateTime DateOfBirth { get; set; }
	public DateTime AccountCreationDate { get; set; } = DateTime.Now;
	public DateTime LastActiveDate { get; set; }
	public bool IsDeleted { get; set; }
}

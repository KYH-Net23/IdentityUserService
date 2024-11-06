namespace IdentityService.Models.ResponseModels;

public class CustomerRequestResponse
{
	public string Id { get; set; } = null!;
	public string? StreetAddress { get; set; }
	public string? City { get; set; }
	public DateTime DateOfBirth { get; set; }
	public string Email { get; set; } = null!;
	public string? PhoneNumber { get; set; }
	public string Username { get; set; } = null!;
	public bool IsDeleted { get; set; }
}

using IdentityService.Infrastructure;

namespace IdentityService.Models.ResponseModels;

public class CustomerRequestResponse
{
	public string Id { get; set; } = null!;
	public string StreetAddress { get; set; } = null!;
	public string City { get; set; } = null!;
	public DateTime DateOfBirth { get; set; }
	public string Email { get; set; } = null!;
	public string PhoneNumber { get; set; } = null!;
	public string PostalCode { get; set; } = null!;
	public string Username { get; set; } = null!;
	public bool IsDeleted { get; set; }
	public string UserRole => UserRoles.Customer.ToString();
}

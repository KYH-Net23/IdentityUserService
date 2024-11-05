namespace IdentityService.Models;

public class CustomerRequestResponse
{
    public string Id { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Username { get; set; }
}

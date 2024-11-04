using System.ComponentModel.DataAnnotations;

namespace IdentityService.Controllers;

public class CreateCustomerModel
{
    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "User name must be between 5 and 50 characters")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 50 characters")]
    [RegularExpression(@"^[\w.-]+@[a-zA-Z\d.-]+.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    public bool EmailConfirmed { get; set; } = true;

    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 50 characters")]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Phone number must be between 5 and 50 characters")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 50 characters")]
    public string Address { get; set; } = null!;
}
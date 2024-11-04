using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class Customer : IdentityUser
{
    // Add extra

    [StringLength(200, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 200 characters")]
    public string Address { get; set; } = null!;
}
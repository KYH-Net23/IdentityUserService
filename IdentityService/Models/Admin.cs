using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class Admin : IdentityUser
{
    // Add extra
    public string Test { get; set; } = null!;
}
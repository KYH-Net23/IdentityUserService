using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class Admin : IdentityUser
{
    public bool IsDeleted { get; set; }
}
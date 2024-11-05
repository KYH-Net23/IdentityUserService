using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public enum AdminLevel
{
    SuperAdmin,
    Admin,
    Moderator
}

public class Admin : IdentityUser
{
    [Range(0, 2)]
    public AdminLevel AdminLevel { get; set; }
    public bool IsDeleted { get; set; }
}
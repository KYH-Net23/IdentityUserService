using System.ComponentModel.DataAnnotations;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models.DataModels;

public class AdminEntity : IdentityUser
{
    [Range(0, 2)]
    public AdminLevel AdminLevel { get; set; }
    public bool IsDeleted { get; set; }
    public bool? HasRequestedPasswordReset { get; set; }
}

using IdentityService.Factories;
using IdentityService.Infrastructure;
using IdentityService.Models.DataModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class AdminService(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager
)
{
    public async Task<List<AdminRequestResponse>> GetAdmins()
    {
        var listOfAdmins = await userManager.GetUsersInRoleAsync(UserRoles.Admin.ToString());

        var adminList = listOfAdmins.OfType<Admin>().ToList();

        return AdminRequestResponseFactory.Create(adminList);
    }
}

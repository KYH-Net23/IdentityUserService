using IdentityService.Factories;
using IdentityService.Infrastructure;
using IdentityService.Models.DataModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class AdminService
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AdminRequestResponse>> GetAdmins()
    {
        var listOfAdmins = await _userManager.GetUsersInRoleAsync(UserRoles.Admin.ToString());

        var adminList = listOfAdmins.OfType<AdminEntity>().ToList();

        return AdminRequestResponseFactory.Create(adminList);
    }
}

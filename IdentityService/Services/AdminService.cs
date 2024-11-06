using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class AdminService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
{
	// Add methods to AdminController here
}

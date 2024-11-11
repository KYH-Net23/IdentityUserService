using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "v2")]
public class AdminController(UserManager<IdentityUser> userManager, AdminService adminService) : ControllerBase
{
	[HttpGet("{email}")]
	public async Task<IActionResult> GetUser(string email)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user != null)
		{
			return Ok(user.Email + user.UserName);
		}

		return BadRequest();
	}

	[HttpGet("GetAdmins")]
	public async Task<IActionResult> GetAdmins()
	{
		var listOfAdmins = await adminService.GetAdmins();
		return Ok(listOfAdmins);
	}

	[HttpDelete("delete")]
	public async Task<IActionResult> DeleteUser()
	{
		var userRole = HttpContext.Items["UserRole"] as string;
		var isAuthenticated = HttpContext.Items["IsAuthenticated"] as bool? ?? false;

		if (userRole != "Admin" || !isAuthenticated)
		{
			return Forbid();
		}
		return Ok("Ok. Not implemented yet.");
	}
}


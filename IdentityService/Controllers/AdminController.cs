using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "v2")]
public class AdminController(UserManager<IdentityUser> userManager) : ControllerBase
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

	[HttpGet("")]
	public async Task<IActionResult> GetUsers()
	{
		return Ok("List of users");
	}

	[HttpDelete("delete/{userId}")]
	public async Task<IActionResult> DeleteUser(string userId)
	{
		return Ok("Ok. Not implemented yet.");
	}
}


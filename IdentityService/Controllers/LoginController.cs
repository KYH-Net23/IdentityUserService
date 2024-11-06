using IdentityService.Models.FormModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class LoginController(UserService userService) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(new { Message = "Model state invalid" });
		}

		var result = await userService.Login(loginModel);

		if (result.Succeeded)
		{
			return Ok(new { result.Message, result.Content });
		}

		return BadRequest(new { result.Message });
	}
}
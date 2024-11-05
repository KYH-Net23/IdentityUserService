using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class CustomerLoginController(CustomerService customerService) : ControllerBase
{
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(new { Message = "Model state invalid" });
		}

		var result = await customerService.CustomerLogin(loginModel);

		if (result.Succeeded)
		{
			return Ok(new { result.Message, result.Content });
		}

		return BadRequest(new { result.Message });
	}
}
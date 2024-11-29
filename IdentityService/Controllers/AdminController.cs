using Asp.Versioning;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AdminService _adminService;

    public AdminController(UserManager<IdentityUser> userManager, AdminService adminService)
    {
        _userManager = userManager;
        _adminService = adminService;
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return Ok(user.Email + user.UserName);
        }

        return BadRequest();
    }

    [HttpGet("GetAdmins")]
    public async Task<IActionResult> GetAdmins()
    {
        var listOfAdmins = await _adminService.GetAdmins();
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

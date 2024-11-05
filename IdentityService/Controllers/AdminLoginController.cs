using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "v2")]
public class AdminLoginController(AdminService adminService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Modelstate invalid");
        }
        
        var result = await adminService.AdminLogin(loginModel);
        
        if (result.Succeeded)
        {
            return Ok(result.Message);
        }
        
        return BadRequest(result.Message);
    }
}
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "v2")]
public class AdminLoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modelstate invalid");
            }

            var loggedInUser = await userManager.FindByEmailAsync(loginModel.Email);
        
            if(loggedInUser == null)
            {
                return BadRequest("User not found");
            }
        
            var tryToSignIn = await signInManager.PasswordSignInAsync(loggedInUser.UserName!, loginModel.Password, false, false);

            if (!tryToSignIn.Succeeded)
            {
                return BadRequest("Login failed");
            }

            var userRole = await userManager.GetRolesAsync(loggedInUser!);

            return Ok(new
            {
                loggedInUser!.Id,
                loggedInUser.Email,
                Roles = userRole
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
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
    public async Task<IActionResult> Login([FromBody] LoginRequest loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Modelstate invalid");
        }

        var tryToSignIn = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);

        if (!tryToSignIn.Succeeded)
        {
            return BadRequest("Success: " + tryToSignIn);
        }

        var loggedInUser = await userManager.FindByEmailAsync(loginModel.Email);
        var userRole = await userManager.GetRolesAsync(loggedInUser!);

        return Ok(new
        {
            loggedInUser!.Id,
            loggedInUser.Email,
            Roles = userRole
        });
    }
}
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class CustomerLoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var tryToSignIn = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);

        if (tryToSignIn.Succeeded)
        {
            return Ok("User logged in");
            // Call on token service to create JWT
        }

        return BadRequest();
    }

    //Only use this for seeding purposes, remove later
    [HttpPost("create")]
    public async Task Create()
    {
        var newUser = new Customer
        {
            UserName = "customer@customer.se",
            NormalizedUserName = "customer@customer.se".ToUpper(),
            Email = "customer@customer.se",
            NormalizedEmail = "customer@customer.se".ToUpper(),
            EmailConfirmed = true,
            Address = "Tom adress"
        };

        await userManager.CreateAsync(newUser, "Hejsan123#");
    }
}
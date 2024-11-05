using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class CustomerLoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
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
            StreetAddress = "Tom adress"
        };

        await userManager.CreateAsync(newUser, "Hejsan123#");
    }
}
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
    public async Task<bool> Login([FromBody] LoginRequest loginModel)
    {
        if (!ModelState.IsValid)
        {
            return false;
        }

        var tryToSignIn = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);

        return tryToSignIn.Succeeded;
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
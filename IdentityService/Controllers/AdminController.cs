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

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateCustomerModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newCustomer = new Customer
        {
            UserName = model.Username,
            NormalizedUserName = model.Username.ToUpper(),
            Email = model.Email,
            NormalizedEmail = model.Email.ToUpper(),
            PhoneNumber = model.PhoneNumber,
            StreetAddress = model.Address
        };

        await userManager.CreateAsync(newCustomer, model.Password);
        await userManager.AddToRoleAsync(newCustomer, UserRoles.Customer.ToString());

        return Ok("User created");
    }

    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        return Ok("Ok. Not implemented yet.");
    }
}

public enum UserRoles
{
    Admin,
    Customer,
    Debug
}
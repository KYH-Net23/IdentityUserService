using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("/[controller]/[action]")]
[ApiController]
public class CustomerController(CustomerService customerService, UserService userService)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IEnumerable<CustomerRequestResponse>> GetCustomers()
    {
        return await customerService.GetDemoCustomers();
    }

    [HttpPost("")]
    public async Task<IActionResult> Register(
        [FromBody] CreateCustomerRequestModel registerRequestModel
    )
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(new { errors });
        }
        try
        {
            var result = await customerService.RegisterCustomer(registerRequestModel);

            if (!result.Succeeded)
                return BadRequest(new { result.Message, PasswordErrors = result.Content });

            // TODO Send request to verification provider here
            return Ok(new { result.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { e.Message });
        }
    }

    [HttpPut("")]
    public async Task<IActionResult> ConfirmEmail([FromBody] UpdateEmailRequest model)
    {
        // Call on auth provider

        var authorizationResult = "await"; // TODO add token provider call here to auth a token

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(new { errors });
        }
        try
        {
            var result = await userService.UpdateEmailConfirmation(model);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

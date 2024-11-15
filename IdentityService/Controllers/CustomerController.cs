using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("/[controller]/[action]")]
[ApiController]
public class CustomerController(CustomerService customerService, AzureEmailSender azureEmailSender)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IEnumerable<CustomerRequestResponse>> GetCustomers()
    {
        return await customerService.GetDemoCustomers();
    }

    [HttpPost("")]
    public async Task<IActionResult> Register([FromBody] CreateCustomerModel registerModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(new { errors });
        }
        try
        {
            var result = await customerService.RegisterCustomer(registerModel);

            if (!result.Succeeded)
                return BadRequest(new { result.Message, PasswordErrors = result.Content });

            await azureEmailSender.SendConfirmationLinkAsync(result.Content as IdentityUser);
            return Ok(new { result.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { e.Message });
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> Demo(EmailRequestModel emailRequestModel)
    {
        await azureEmailSender.TestMethod(emailRequestModel);
        return Ok();
    }
}

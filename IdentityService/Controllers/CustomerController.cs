using Asp.Versioning;
using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomerController(CustomerService customerService, AzureEmailSender azureEmailSender)
    : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IEnumerable<CustomerRequestResponse>> GetCustomers()
    {
        return await customerService.GetDemoCustomers();
    }

    [HttpPost("[action]")]
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

            await azureEmailSender.Execute(result);
            return Ok(new { result.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { e.Message });
        }
    }
}

using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using IdentityService.Services.HttpClientServices;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("/[controller]/[action]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly CustomerService _customerService;
    private readonly UserService _userService;
    private readonly VerificationHttpClient _verificationHttpClient;

    public CustomerController(
        CustomerService customerService,
        UserService userService,
        VerificationHttpClient verificationHttpClient
    )
    {
        _customerService = customerService;
        _userService = userService;
        _verificationHttpClient = verificationHttpClient;
    }

    [HttpGet("")]
    public async Task<IEnumerable<CustomerRequestResponse>> GetCustomers()
    {
        return await _customerService.GetDemoCustomers();
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
            var result = await _customerService.RegisterCustomer(registerRequestModel);

            if (!result.Succeeded)
                return BadRequest(new { result.Message, PasswordErrors = result.Content });

            await _verificationHttpClient.PostAsync((result.Content as EmailRequestModel)!);

            if (result.Succeeded)
                return Ok(new { result.Message });

            return BadRequest(new { result.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { e.Message });
        }
    }

    [HttpPut("")]
    public async Task<IActionResult> ConfirmEmail([FromBody] EmailModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(new { errors });
        }
        try
        {
            var result = await _userService.UpdateEmailConfirmation(model);
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

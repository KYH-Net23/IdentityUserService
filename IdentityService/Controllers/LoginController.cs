using Azure.Communication.Email;
using IdentityService.Models.RequestModels;
using IdentityService.Services;
using IdentityService.Services.HttpClientServices;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly UserService _userService;
    private readonly VerificationHttpClient _verificationService;

    public LoginController(UserService userService, VerificationHttpClient verificationService)
    {
        _userService = userService;
        _verificationService = verificationService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Model state invalid" });

        var result = await _userService.Login(loginRequestModel);

        if (result.Succeeded)
            return Ok(new { result.Message, result.Content });

        if (result is not { Succeeded: false, Content: EmailRequestModel emailRequestModel })
            return BadRequest(new { result });

        var responseMessage = await _verificationService.PostAsync(emailRequestModel);

        if (responseMessage.IsSuccessStatusCode)
            return Ok(new { Message = "Redirect the user to the authorization page." });

        return BadRequest(new { result });
    }
}

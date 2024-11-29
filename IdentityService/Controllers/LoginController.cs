using IdentityService.Models.RequestModels;
using IdentityService.Services;
using IdentityService.Services.HttpClientServices;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController(UserService userService, VerificationHttpClient verificationService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Model state invalid" });

        var result = await userService.Login(loginRequestModel);

        if (result.Succeeded)
            return Ok(new { result.Message, result.Content });

        if (result is not { Succeeded: false, Content: EmailRequestModel emailRequestModel })
            return BadRequest(new { result });

        var responseMessage = await verificationService.PostAsync(emailRequestModel);

        if (responseMessage.IsSuccessStatusCode)
            return Ok(new { Message = "Go to the auth site" }); // TODO better message here

        return BadRequest(new { result });
    }
}

using IdentityService.Models.FormModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController(UserService userService, VerificationHttpClient verificationService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = "Model state invalid" });
        }

        var result = await userService.Login(loginModel);

        if (result.Succeeded)
        {
            return Ok(new { result.Message, result.Content });
        }

        if (result is not { Succeeded: false, Content: EmailRequestModel })
            return BadRequest(new { result });

        var responseMessage = await verificationService.PostAsync(
            result.Content as EmailRequestModel
        );

        if (responseMessage.IsSuccessStatusCode)
        {
            return Ok(new { Message = "Go to the auth site" });
        }

        return BadRequest(new { result });
    }
}

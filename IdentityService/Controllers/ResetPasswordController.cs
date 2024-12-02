using IdentityService.Models.RequestModels;
using IdentityService.Services;
using IdentityService.Services.HttpClientServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailProviderHttpClient _emailProviderHttpClient;

        public ResetPasswordController(
            UserService userService,
            EmailProviderHttpClient emailProviderHttpClient
        )
        {
            _userService = userService;
            _emailProviderHttpClient = emailProviderHttpClient;
        }

        [HttpPost("/send-reset-password-email")]
        public async Task<IActionResult> GetUserId([FromBody] EmailModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _userService.FindUserByEmailAsync(model.Email);
                if (result == null)
                    return NotFound();

                await _emailProviderHttpClient.PostAsync("/reset", result);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/reset")]
        public async Task<IActionResult> ResetPasswordRequest(
            [FromBody] PasswordResetRequestModel model
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.ResetPassword(model.ResetGuid);
                if (result.Succeeded)
                    return Ok();

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/change-password")]
        public async Task<IActionResult> ChangePasswordRequest(
            [FromBody] ChangePasswordRequestModel requestModel
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _userService.ChangePassword(requestModel);
                if (result.Succeeded)
                    return Ok();

                return BadRequest(result.Errors.Select(x => x.Description));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

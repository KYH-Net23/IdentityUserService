using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailProviderHttpClient _emailProviderHttpClient;

        public ResetPasswordController(UserService userService, EmailProviderHttpClient emailProviderHttpClient)
        {
            _userService = userService;
            _emailProviderHttpClient = emailProviderHttpClient;
        }

        [HttpPost("")]
        public async Task<IActionResult> GetUserId([FromBody] string email)
        {
            try
            {
                var result = await _userService.FindUserByEmailAsync(email);
                if (result == null)
                {
                    return NotFound();
                }

                else
                    await _emailProviderHttpClient.PostAsync("/reset", result);
                    

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }         
        }


        [HttpPost("/reset")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] string passwordRequestGuid)
        {
            try
            {
                var result = await _userService.ResetPassword(passwordRequestGuid);
                if (result)
                {
                    return Ok();
                }
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/changepassword")]
        public async Task<IActionResult> ChangePasswordRequest([FromBody] ChangePasswordModel model)
        {
            try
            {
                var result = await _userService.ChangePassword(model);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

using IdentityService.Controllers;
using IdentityService.Models.DataModels;
using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class UserService(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager
)
{
    public async Task<ResponseResult> Login(
        LoginRequestModel loginRequestModel,
        bool rememberMe = false
    )
    {
        try
        {
            var loggedInUser = await userManager.FindByEmailAsync(loginRequestModel.Email);

            if (loggedInUser == null)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = $"User \"{loginRequestModel.Email}\" not found",
                    Content = $"User \"{loginRequestModel.Email}\" not found"
                };
            }

            var tryToSignIn = await signInManager.PasswordSignInAsync(
                loggedInUser.UserName!,
                loginRequestModel.Password,
                rememberMe,
                false
            );

            if (tryToSignIn.IsNotAllowed)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message =
                        $"Your email (\"{loginRequestModel.Email}\") is not confirmed. Please check your email for instructions.",
                    Content = new EmailRequestModel
                    {
                        EmailAddress = loginRequestModel.Email,
                        UserId = loginRequestModel.Email // TODO change to ID later
                    }
                };
            }

            if (!tryToSignIn.Succeeded)
            {
                await userManager.AccessFailedAsync(loggedInUser);
                if (!await userManager.IsLockedOutAsync(loggedInUser))
                    return new ResponseResult
                    {
                        Succeeded = false,
                        Message = "Invalid login attempt",
                        Content = "Invalid login attempt"
                    };
                var lockoutEndDate = await userManager.GetLockoutEndDateAsync(loggedInUser);
                return new ResponseResult
                {
                    Succeeded = false,
                    Message =
                        $"Too many attempts. Account temporarily locked until {lockoutEndDate}",
                    Content =
                        $"Too many attempts. Account temporarily locked until {lockoutEndDate}"
                };
            }

            var userRole = await userManager.GetRolesAsync(loggedInUser);
            if (loggedInUser is CustomerEntity customer)
            {
                customer.LastActiveDate = DateTime.Now;
                await userManager.UpdateAsync(customer);
                return new ResponseResult
                {
                    Succeeded = true,
                    Message = "Login successful",
                    Content = new
                    {
                        Role = userRole,
                        customer.DateOfBirth,
                        customer.StreetAddress,
                        customer.City,
                        customer.PostalCode,
                        customer.Email,
                        customer.PhoneNumber,
                    }
                };
            }

            return new ResponseResult
            {
                Succeeded = true,
                Message = "Login successful",
                Content = new
                {
                    Role = userRole,
                    loginRequestModel.Email,
                    loggedInUser.Id
                }
            };
        }
        catch (Exception e)
        {
            return new ResponseResult
            {
                Succeeded = false,
                Message = e.Message,
                Content = e.Message
            };
        }
    }

    public async Task<ResponseResult> UpdateEmailConfirmation(UpdateEmailRequest model)
    {
        // Verification provider calls on this
        try
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "Email address not found",
                    Content = "Email address not found"
                };
            }

            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
            return new ResponseResult
            {
                Succeeded = true,
                Message = "Email address confirmed",
                Content = "Email address confirmed"
            };
        }
        catch { }

        return new ResponseResult
        {
            Succeeded = false,
            Message = null,
            Content = null
        };
    }
}

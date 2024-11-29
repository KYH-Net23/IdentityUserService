using IdentityService.Controllers;
using IdentityService.Data;
using IdentityService.Models.DataModels;
using IdentityService.Models.FormModels;
using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class UserService(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    DataContext dbContext
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

    public async Task<ResetPasswordModel?> FindUserByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }
        if (user is CustomerEntity customer)
        {
            customer.PasswordResetGuid = Guid.NewGuid().ToString();
            await userManager.UpdateAsync(customer);
            return new ResetPasswordModel
            {
                Receiver = email,
                ResetGuid = customer.PasswordResetGuid
            };
        }
        else if (user is AdminEntity admin)
        {
            admin.PasswordResetGuid = Guid.NewGuid().ToString();
            await userManager.UpdateAsync(admin);
            return new ResetPasswordModel
            {
                Receiver = email,
                ResetGuid = admin.PasswordResetGuid
            };
        }

        return null!;
    }


    public async Task<bool> ResetPassword(string guid)
    {
        var user = await dbContext.Customers.FirstOrDefaultAsync(x => x.PasswordResetGuid == guid);

        if (user != null)
        {
            await userManager.SetLockoutEndDateAsync(user!, DateTime.Now.AddHours(2));
            user.HasRequestedPasswordReset = true;
            await userManager.UpdateAsync(user);
            return true;
        }
        return false;
    }

    public async Task<IdentityResult> ChangePassword(ChangePasswordModel model)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null && user is CustomerEntity customer)
            {
                if(customer.HasRequestedPasswordReset == null)
                {
                    return IdentityResult.Failed();
                }
                else
                {
                    await userManager.RemovePasswordAsync(user!);
                    var result = await userManager.AddPasswordAsync(user!, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user!.LockoutEnd = null;
                        customer.HasRequestedPasswordReset = null;
                        await userManager.UpdateAsync(customer);
                    }
                    return result;
                }
            }
            return IdentityResult.Failed();
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }

}

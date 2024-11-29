﻿using IdentityService.Models.DataModels;
using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class UserService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private const int LockOutDurationInHours = 2;

    public UserService(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager
    )
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<ResponseResult> Login(
        LoginRequestModel loginRequestModel,
        bool rememberMe = false
    )
    {
        try
        {
            var loggedInUser = await _userManager.FindByEmailAsync(loginRequestModel.Email);

            if (loggedInUser == null)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = $"User {loginRequestModel.Email} not found",
                    Content = $"User {loginRequestModel.Email} not found"
                };
            }

            var tryToSignIn = await _signInManager.PasswordSignInAsync(
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
                        $"Your email ({loginRequestModel.Email}) is not confirmed. Please check your email for instructions.",
                    Content = new EmailRequestModel
                    {
                        EmailAddress = loginRequestModel.Email,
                        UserId = loginRequestModel.Email // TODO change to ID later
                    }
                };
            }

            if (!tryToSignIn.Succeeded)
            {
                await _userManager.AccessFailedAsync(loggedInUser);
                if (!await _userManager.IsLockedOutAsync(loggedInUser))
                    return new ResponseResult
                    {
                        Succeeded = false,
                        Message = "Invalid login attempt",
                        Content = "Invalid login attempt"
                    };

                var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(loggedInUser);

                if (loggedInUser is not CustomerEntity customerEntity)
                    return new ResponseResult
                    {
                        Succeeded = false,
                        Message =
                            $"Too many attempts. Account temporarily locked until {lockoutEndDate}",
                        Content =
                            $"Too many attempts. Account temporarily locked until {lockoutEndDate}"
                    };

                if (
                    customerEntity.HasRequestedPasswordReset != null
                    && (bool)customerEntity.HasRequestedPasswordReset
                )
                {
                    return new ResponseResult
                    {
                        Succeeded = false,
                        Message =
                            "You have requested a password reset. Please reset your password before trying to log in.",
                        Content =
                            "You have requested a password reset. Please reset your password before trying to log in."
                    };
                }

                return new ResponseResult
                {
                    Succeeded = false,
                    Message =
                        $"Too many attempts. Account temporarily locked until {lockoutEndDate}",
                    Content =
                        $"Too many attempts. Account temporarily locked until {lockoutEndDate}"
                };
            }

            var userRole = await _userManager.GetRolesAsync(loggedInUser);

            if (loggedInUser is not CustomerEntity customer)
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

            customer.LastActiveDate = DateTime.Now;
            await _userManager.UpdateAsync(customer);

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
                    customer.PhoneNumber
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

    public async Task<ResponseResult> UpdateEmailConfirmation(
        AuthorizationForEmailProviderRequestModel model
    )
    {
        // Verification provider calls on this
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
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
            await _userManager.UpdateAsync(user);
            return new ResponseResult
            {
                Succeeded = true,
                Message = "Email address confirmed",
                Content = "Email address confirmed"
            };
        }
        catch
        {
            return new ResponseResult
            {
                Succeeded = false,
                Message = null,
                Content = null
            };
        }
    }

    public async Task<ResetPasswordModel?> FindUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        switch (user)
        {
            case null:
                return null;
            case CustomerEntity customer:
                customer.PasswordResetGuid = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(customer);
                return new ResetPasswordModel
                {
                    Receiver = email,
                    ResetGuid = customer.PasswordResetGuid
                };
            case AdminEntity:
                return null;
            default:
                return null!;
        }
    }

    public async Task<ResponseResult> ResetPassword(string guid)
    {
        try
        {
            var user = await _userManager
                .Users.Cast<CustomerEntity>()
                .FirstOrDefaultAsync(x => x.PasswordResetGuid == guid);

            if (user == null)
                return new ResponseResult { Succeeded = false, Message = "User does not exist." };

            await _userManager.SetLockoutEndDateAsync(
                user,
                DateTime.Now.AddHours(LockOutDurationInHours)
            );
            user.HasRequestedPasswordReset = true;
            await _userManager.UpdateAsync(user);

            return new ResponseResult { Succeeded = true };
        }
        catch (InvalidCastException e)
        {
            return new ResponseResult
            {
                Succeeded = false,
                Message = "Admins can not reset their passwords.",
                Content = e.Message
            };
        }
        catch (Exception e)
        {
            return new ResponseResult
            {
                Succeeded = false,
                Message = null,
                Content = e.Message
            };
        }
    }

    public async Task<IdentityResult> ChangePassword(ChangePasswordRequestModel requestModel)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(requestModel.Email);

            if (user is not CustomerEntity customer || customer.HasRequestedPasswordReset is null)
                return IdentityResult.Failed();

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, requestModel.NewPassword);

            if (!result.Succeeded)
                return result;

            user.LockoutEnd = null;
            customer.HasRequestedPasswordReset = null;
            customer.PasswordResetGuid = null;
            await _userManager.UpdateAsync(customer);

            return result;
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }
}

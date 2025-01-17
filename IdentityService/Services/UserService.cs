﻿using IdentityService.Models.DataModels;
using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            if (!await _userManager.IsEmailConfirmedAsync(loggedInUser))
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "User email is not confirmed.",
                    Content = new EmailRequestModel
                    {
                        EmailAddress = loginRequestModel.Email,
                        UserId = loginRequestModel.Email
                    }
                };
            }

            var tryToSignIn = await _signInManager.PasswordSignInAsync(
                loggedInUser.UserName!,
                loginRequestModel.Password,
                rememberMe,
                false
            );

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

            if (loggedInUser is not CustomerEntity customer) // User is admin
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

    public async Task<ResponseResult> UpdateEmailConfirmation(EmailModel model)
    {
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
            var customer = await _userManager
                .Users.Cast<CustomerEntity>()
                .FirstOrDefaultAsync(x => x.PasswordResetGuid == requestModel.Id);

            if (customer?.HasRequestedPasswordReset is null)
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UserDoesNotExist",
                        Description = "The user does not exist."
                    }
                );

            await _userManager.RemovePasswordAsync(customer);
            var result = await _userManager.AddPasswordAsync(customer, requestModel.NewPassword);

            if (!result.Succeeded)
                return IdentityResult.Failed(result.Errors.ToArray());

            customer.LockoutEnd = null;
            customer.HasRequestedPasswordReset = null;
            customer.PasswordResetGuid = null;

            await _userManager.UpdateAsync(customer);

            return result;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed(
                new IdentityError { Code = e.Message, Description = e.Message }
            );
        }
    }
}

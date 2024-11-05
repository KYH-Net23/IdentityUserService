﻿using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class AdminService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
{
    public async Task<ResponseResult> AdminLogin(LoginModel loginModel)
    {
        try
        {
            var loggedInUser = await userManager.FindByEmailAsync(loginModel.Email);
        
            if(loggedInUser == null)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "User not found"
                };
            }
        
            var tryToSignIn = await signInManager.PasswordSignInAsync(loggedInUser.UserName!, loginModel.Password, false, false);

            if (!tryToSignIn.Succeeded)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "Login failed"
                };
            }

            var userRole = await userManager.GetRolesAsync(loggedInUser!);

            return new ResponseResult
            {
                Succeeded = true,
                Message = "Login successful",
                Content = new
                {
                    loggedInUser.Id,
                    loggedInUser.Email,
                    Roles = userRole
                }
            };

        }
        catch (Exception e)
        {
            return new ResponseResult
            {
                Succeeded = false,
                Message = e.Message
            };
        }
    }
}

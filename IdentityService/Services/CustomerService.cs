using IdentityService.Controllers;
using IdentityService.Data;
using IdentityService.Factory;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class CustomerService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
{
    public async Task<List<CustomerRequestResponse>> GetDemoCustomers()
    {
        var listOfCustomers = await userManager.GetUsersInRoleAsync(UserRoles.Customer.ToString());

        return CustomerRequestResponseFactory.Create((listOfCustomers as IList<Customer>)!);
    }

    public async Task<ResponseResult> CustomerLogin(LoginModel loginModel)
    {
        try
        {
            var loggedInUser = await userManager.FindByEmailAsync(loginModel.Email);

            if (loggedInUser == null)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "User not found"
                };
            }

            var tryToSignIn =
                await signInManager.PasswordSignInAsync(loggedInUser.UserName!, loginModel.Password, false, false);

            if (!tryToSignIn.Succeeded)
            {
                var counter = await userManager.GetAccessFailedCountAsync(loggedInUser);
                await userManager.AccessFailedAsync(loggedInUser);
                if (!await userManager.IsLockedOutAsync(loggedInUser))
                    return new ResponseResult
                    {
                        Succeeded = false,
                        Message = "Login failed"
                    };
                var lockoutEndDate = await userManager.GetLockoutEndDateAsync(loggedInUser);
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = $"Too many attempts. Account temporarily locked until {lockoutEndDate}"
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

    public async Task<ResponseResult> RegisterCustomer(CreateCustomerModel registerModel)
    {
        try
        {
            var existingUserName = await userManager.FindByNameAsync(registerModel.Username);
            var existingEmail = await userManager.FindByEmailAsync(registerModel.Email);

            if (existingUserName != null || existingEmail != null)
            {
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "User already exists"
                };
            }

            var newCustomer = new Customer
            {
                UserName = registerModel.Username,
                NormalizedUserName = registerModel.Username.ToUpper(),
                Email = registerModel.Email,
                NormalizedEmail = registerModel.Email.ToUpper(),
                PhoneNumber = registerModel.PhoneNumber,
                StreetAddress = registerModel.StreetAddress,
                DateOfBirth = registerModel.DateOfBirth,
                City = registerModel.City,
                AccountCreationDate = registerModel.AccountCreationDate,
                LastActiveDate = registerModel.LastActiveDate
            };

            await userManager.CreateAsync(newCustomer, registerModel.Password);
            await userManager.AddToRoleAsync(newCustomer, UserRoles.Customer.ToString());

            return new ResponseResult
            {
                Succeeded = true,
                Message = "User created"
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
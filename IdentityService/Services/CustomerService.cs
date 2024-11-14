using IdentityService.Extensions;
using IdentityService.Factory;
using IdentityService.Infrastructure;
using IdentityService.Models.DataModels;
using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomerService(UserManager<IdentityUser> userManager)
{
    public async Task<List<CustomerRequestResponse>> GetDemoCustomers()
    {
        var listOfCustomers = await userManager.GetUsersInRoleAsync(UserRoles.Customer.ToString());

        var customerList = listOfCustomers.OfType<Customer>().ToList();

        return CustomerRequestResponseFactory.Create(customerList);
    }

    public async Task<ResponseResult> RegisterCustomer(CreateCustomerModel registerModel)
    {
        try
        {
            var existingUserName = await userManager.FindByNameAsync(registerModel.Username);
            var existingEmail = await userManager.FindByEmailAsync(registerModel.Email);

            if (existingUserName != null || existingEmail != null)
            {
                return new ResponseResult { Succeeded = false, Message = "User already exists" };
            }

            var newCustomer = registerModel.MapToCustomer();
            var result = await userManager.CreateAsync(newCustomer, registerModel.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new ResponseResult
                {
                    Succeeded = false,
                    Message = "User creation failed",
                    Content = errors
                };
            }
            await userManager.AddToRoleAsync(newCustomer, UserRoles.Customer.ToString());

            return new ResponseResult
            {
                Succeeded = true,
                Message = "User created",
                Content = userManager.FindByEmailAsync(registerModel.Email)
            };
        }
        catch (Exception e)
        {
            return new ResponseResult { Succeeded = false, Message = e.Message };
        }
    }
}

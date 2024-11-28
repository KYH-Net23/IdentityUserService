using IdentityService.Extensions;
using IdentityService.Factories;
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

        var customerList = listOfCustomers.OfType<CustomerEntity>().ToList();

        return CustomerRequestResponseFactory.Create(customerList);
    }

    public async Task<ResponseResult> RegisterCustomer(
        CreateCustomerRequestModel registerRequestModel
    )
    {
        try
        {
            var existingUserName = await userManager.FindByNameAsync(registerRequestModel.Username);
            var existingEmail = await userManager.FindByEmailAsync(registerRequestModel.Email);

            if (existingUserName != null || existingEmail != null)
            {
                return new ResponseResult { Succeeded = false, Message = "User already exists" };
            }

            var newCustomer = registerRequestModel.MapToCustomer();
            var result = await userManager.CreateAsync(newCustomer, registerRequestModel.Password);

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
                Content = userManager.FindByEmailAsync(registerRequestModel.Email)
            };
        }
        catch (Exception e)
        {
            return new ResponseResult { Succeeded = false, Message = e.Message };
        }
    }
}

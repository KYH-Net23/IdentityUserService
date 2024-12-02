using IdentityService.Extensions;
using IdentityService.Factories;
using IdentityService.Infrastructure;
using IdentityService.Models.DataModels;
using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomerService
{
    private readonly UserManager<IdentityUser> _userManager;

    public CustomerService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<CustomerRequestResponse>> GetDemoCustomers()
    {
        var listOfCustomers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer.ToString());

        var customerList = listOfCustomers.OfType<CustomerEntity>().ToList();

        return CustomerRequestResponseFactory.Create(customerList);
    }

    public async Task<ResponseResult> RegisterCustomer(
        CreateCustomerRequestModel registerRequestModel
    )
    {
        try
        {
            var existingUserName = await _userManager.FindByNameAsync(
                registerRequestModel.Username
            );
            var existingEmail = await _userManager.FindByEmailAsync(registerRequestModel.Email);

            if (existingUserName != null || existingEmail != null)
            {
                return new ResponseResult { Succeeded = false, Message = "User already exists" };
            }

            var newCustomer = registerRequestModel.MapToCustomer();
            var result = await _userManager.CreateAsync(newCustomer, registerRequestModel.Password);

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
            await _userManager.AddToRoleAsync(newCustomer, UserRoles.Customer.ToString());

            var customer = await _userManager.FindByEmailAsync(registerRequestModel.Email);

            return new ResponseResult
            {
                Succeeded = true,
                Message = "User created",
                Content = new EmailRequestModel
                {
                    EmailAddress = customer.Email,
                    UserId = customer.Id
                }
            };
        }
        catch (Exception e)
        {
            return new ResponseResult { Succeeded = false, Message = e.Message };
        }
    }
}

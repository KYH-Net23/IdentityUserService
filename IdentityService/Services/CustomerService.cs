using IdentityService.Controllers;
using IdentityService.Data;
using IdentityService.Factory;
using IdentityService.Infrastructure;
using IdentityService.Models;
using IdentityService.Models.DataModels;
using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
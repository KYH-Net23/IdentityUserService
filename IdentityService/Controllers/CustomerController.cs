using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomerController(CustomerService customerService) : ControllerBase
{
	[ApiExplorerSettings(GroupName = "v2")]
	[HttpGet("[action]")]
	public async Task<IEnumerable<CustomerRequestResponse>> GetCustomers()
	{
		return await customerService.GetDemoCustomers();
	}


	[ApiExplorerSettings(GroupName = "v2")]
	[HttpPost("[action]")]
	public async Task<IActionResult> Register([FromBody] CreateCustomerModel registerModel)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors);
				return BadRequest(new {errors});
			}

			var result = await customerService.RegisterCustomer(registerModel);

			if (result.Succeeded)
			{
				return Ok(new {result.Message});
			}

			return BadRequest(new {result});
		}
		catch (Exception e)
		{
			return BadRequest(new{e.Message});
		}
	}
}

using IdentityService.Models.FormModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomerController(CustomerService customerService) : ControllerBase
{
	[ApiExplorerSettings(GroupName = "v1")]
	[HttpGet("[action]")]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await customerService.GetDemoCustomers());
	}

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
				return BadRequest("Modelstate invalid");
			}

			var result = await customerService.RegisterCustomer(registerModel);

			if (result.Succeeded)
			{
				return Ok(result.Message);
			}

			return BadRequest(result.Message);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}

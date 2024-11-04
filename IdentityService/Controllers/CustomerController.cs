using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController(CustomerService customerService) : ControllerBase
{
    [ApiExplorerSettings(GroupName = "v1")]
    [HttpGet("[action]")]
    public async Task<IEnumerable<CustomerRequestResponse>> GetAll()
    {
        return await customerService.GetDemoCustomers();
    }

    [ApiExplorerSettings(GroupName = "v2")]
    [HttpGet("[action]")]
    public async Task<IEnumerable<CustomerRequestResponse>> GetNull()
    {
        return null;
    }
}

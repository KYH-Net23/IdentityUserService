using IdentityService.Data;
using IdentityService.Factory;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class CustomerService(DataContext dbContext)
{
    public async Task<List<CustomerRequestResponse>> GetDemoCustomers()
    {
        var listOfCustomers = await dbContext.Customers.ToListAsync();

        return CustomerRequestResponseFactory.Create(listOfCustomers);
    }
}

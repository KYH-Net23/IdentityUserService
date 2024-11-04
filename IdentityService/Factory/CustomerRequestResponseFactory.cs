using IdentityService.Models;

namespace IdentityService.Factory;

public static class CustomerRequestResponseFactory
{
    public static CustomerRequestResponse Create(Customer customer)
    {
        return new CustomerRequestResponse
        {
            Id = customer.Id,
            Address = customer.Address,
            Email = customer.Email!,
            PhoneNumber = customer.PhoneNumber!,
            Username = customer.UserName!
        };
    }

    public static List<CustomerRequestResponse> Create(List<Customer> listOfCustomers)
    {
        return listOfCustomers.Select(Create).ToList();
    }
}

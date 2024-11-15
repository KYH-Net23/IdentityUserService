using IdentityService.Models.DataModels;
using IdentityService.Models.ResponseModels;

namespace IdentityService.Factories;

public static class CustomerRequestResponseFactory
{
    public static CustomerRequestResponse Create(Customer customer)
    {
        return new CustomerRequestResponse
        {
            Id = customer.Id,
            StreetAddress = customer.StreetAddress,
            City = customer.City!,
            DateOfBirth = customer.DateOfBirth,
            Email = customer.Email!,
            PhoneNumber = customer.PhoneNumber!,
            PostalCode = customer.PostalCode,
            Username = customer.UserName!,
            IsDeleted = customer.IsDeleted
        };
    }

    public static List<CustomerRequestResponse> Create(IList<Customer> listOfCustomers)
    {
        return listOfCustomers.Select(Create).ToList();
    }
}

using IdentityService.Models.DataModels;
using IdentityService.Models.ResponseModels;

namespace IdentityService.Factories;

public static class CustomerRequestResponseFactory
{
    public static CustomerRequestResponse Create(CustomerEntity customerEntity)
    {
        return new CustomerRequestResponse
        {
            Id = customerEntity.Id,
            StreetAddress = customerEntity.StreetAddress,
            City = customerEntity.City,
            DateOfBirth = customerEntity.DateOfBirth,
            Email = customerEntity.Email!,
            PhoneNumber = customerEntity.PhoneNumber!,
            PostalCode = customerEntity.PostalCode,
            Username = customerEntity.UserName!,
            IsDeleted = customerEntity.IsDeleted
        };
    }

    public static List<CustomerRequestResponse> Create(IList<CustomerEntity> listOfCustomers)
    {
        return listOfCustomers.Select(Create).ToList();
    }
}

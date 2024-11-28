using IdentityService.Models.DataModels;
using IdentityService.Models.FormModels;

namespace IdentityService.Extensions;

public static class CustomerExtensions
{
    public static CustomerEntity MapToCustomer(this CreateCustomerRequestModel source)
    {
        return new CustomerEntity
        {
            UserName = source.Username,
            NormalizedUserName = source.Username.ToUpper(),
            Email = source.Email,
            NormalizedEmail = source.Email.ToUpper(),
            PhoneNumber = source.PhoneNumber,
            StreetAddress = source.StreetAddress,
            City = source.City,
            DateOfBirth = source.DateOfBirth,
            AccountCreationDate = DateTime.Now,
            PostalCode = source.PostalCode,
            IsDeleted = false
        };
    }
}

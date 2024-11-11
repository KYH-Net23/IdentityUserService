using IdentityService.Models.DataModels;
using IdentityService.Models.FormModels;

namespace IdentityService.Extensions;

public static class CustomerExtensions
{
    public static Customer MapToCustomer(this CreateCustomerModel source)
    {
        return new Customer
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
            IsDeleted = false
        };
    }
}
using IdentityService.Models.DataModels;
using IdentityService.Models.ResponseModels;

namespace IdentityService.Factory;

public static class AdminRequestResponseFactory
{
    public static AdminRequestResponse Create(Admin admin)
    {
        return new AdminRequestResponse
        {
            Id = admin.Id,
            Email = admin.Email!,
            Username = admin.UserName!,
            IsDeleted = admin.IsDeleted
        };
    }

    public static List<AdminRequestResponse> Create(IList<Admin> listOfAdmins)
    {
        return listOfAdmins.Select(Create).ToList();
    }
}
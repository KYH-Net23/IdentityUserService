using IdentityService.Models.DataModels;
using IdentityService.Models.ResponseModels;

namespace IdentityService.Factories;

public static class AdminRequestResponseFactory
{
    public static AdminRequestResponse Create(AdminEntity adminEntity)
    {
        return new AdminRequestResponse
        {
            Id = adminEntity.Id,
            Email = adminEntity.Email!,
            Username = adminEntity.UserName!,
            IsDeleted = adminEntity.IsDeleted
        };
    }

    public static List<AdminRequestResponse> Create(IList<AdminEntity> listOfAdmins)
    {
        return listOfAdmins.Select(Create).ToList();
    }
}

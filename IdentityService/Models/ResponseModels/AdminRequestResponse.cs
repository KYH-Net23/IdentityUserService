using IdentityService.Infrastructure;

namespace IdentityService.Models.ResponseModels;

public class AdminRequestResponse
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public bool IsDeleted { get; set; }

    public string UserRole => UserRoles.Admin.ToString();
}
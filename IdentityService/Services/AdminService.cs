using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class AdminService(DataContext dataContext)
{
    public async Task<IEnumerable<Admin>> GetAdminsAsync()
    {
        var result = await dataContext.Admins.ToListAsync();
        return result;
    }
}

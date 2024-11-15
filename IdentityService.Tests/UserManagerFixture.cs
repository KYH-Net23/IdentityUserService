using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IdentityService.Tests;

public class UserManagerFixture : IDisposable
{
    public Mock<UserManager<IdentityUser>> UserManagerMock { get; set; }
    public CustomerService CustomerService { get; set; }

    public UserManagerFixture()
    {
        UserManagerMock = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );

        CustomerService = new CustomerService(UserManagerMock.Object);
    }

    public void Dispose()
    {
        UserManagerMock.Object.Dispose();
    }
}

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
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<IdentityUser>>().Object,
            Array.Empty<IUserValidator<IdentityUser>>(),
            Array.Empty<IPasswordValidator<IdentityUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<IdentityUser>>>().Object);

        CustomerService = new CustomerService(UserManagerMock.Object);
    }

    public void Dispose()
    {
        UserManagerMock.Object.Dispose();
    }
}
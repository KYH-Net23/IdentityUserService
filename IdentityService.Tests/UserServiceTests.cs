using IdentityService.Models;
using IdentityService.Models.FormModels;
using IdentityService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace IdentityService.Tests;

public class UserServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
            _mockUserManager.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
            null!, null!, null!, null!
        );

        _userService = new UserService(_mockSignInManager.Object, _mockUserManager.Object);
    }
    
    [Fact]
    public async Task Login_Fails_And_LocksOut_After_Max_Attempts()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "test@test.se",
            Password = "illegals"
        };

        var user = new IdentityUser { Email = loginModel.Email, UserName = "tester" };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(user.UserName, loginModel.Password, false, false))
            .ReturnsAsync(SignInResult.Failed);

        _mockUserManager.Setup(m => m.AccessFailedAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.SetupSequence(m => m.GetAccessFailedCountAsync(user))
            .ReturnsAsync(1)
            .ReturnsAsync(2)
            .ReturnsAsync(3);
        _mockUserManager.SetupSequence(m => m.IsLockedOutAsync(user))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);
        _mockUserManager.Setup(m => m.GetLockoutEndDateAsync(user))
            .ReturnsAsync(DateTimeOffset.UtcNow.AddMinutes(10));

        // Act & Assert

        for (var attempts = 1; attempts <= 3; attempts++)
        {
            var result = await _userService.Login(loginModel);

            Assert.False(result.Succeeded);
            if (attempts < 3)
            {
                Assert.Equal("Invalid login attempt", result.Message);
            }
            else
            {
                Assert.Contains("Account temporarily locked", result.Message);
            }
        }
    }
}

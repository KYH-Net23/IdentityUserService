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
    
    [Fact]
    public async Task Login_Succeeds_With_Correct_Credentials()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "admin@test.com",
            Password = "correctpassword"
        };

        var user = new IdentityUser { Email = loginModel.Email, UserName = "adminuser" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(user.UserName, loginModel.Password, false, false))
            .ReturnsAsync(SignInResult.Success);

        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Admin" });

        // Act
        var result = await _userService.Login(loginModel);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Login successful", result.Message);
        Assert.NotNull(result.Content);

        var content = result.Content;
        var idProperty = content.GetType().GetProperty("Id");
        var emailProperty = content.GetType().GetProperty("Email");
        var rolesProperty = content.GetType().GetProperty("Roles");

        Assert.Equal(user.Id, idProperty!.GetValue(content) as string);
        Assert.Equal(user.Email, emailProperty!.GetValue(content) as string);
        Assert.Contains("Admin", (IList<string>)rolesProperty?.GetValue(content)!);
    }
    
    [Fact]
    public async Task Login_Fails_When_User_Does_Not_Exist_In_Database()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "nonexistent@admin.com",
            Password = "fakepassword"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
            .ReturnsAsync((IdentityUser)null!);

        // Act
        var result = await _userService.Login(loginModel);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("User not found", result.Message);
        Assert.Equal("User not found", result.Content);
    }
    
    [Fact]
    public async Task Login_With_RememberMe_Keeps_User_Signed_In()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "admin@test.com",
            Password = "validpassword"
        };

        var user = new IdentityUser { Email = loginModel.Email, UserName = "adminuser" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
            .ReturnsAsync(user);

        const bool rememberMe = true;
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(user.UserName, loginModel.Password, rememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var result = await _userService.Login(loginModel, rememberMe);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Login successful", result.Message);

        _mockSignInManager.Verify(x => x.PasswordSignInAsync(user.UserName, loginModel.Password, rememberMe, false), Times.Once);
    }


}

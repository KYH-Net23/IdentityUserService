using IdentityService.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace IdentityService.Tests;

public class RegisterCustomerTests(UserManagerFixture fixture) : IClassFixture<UserManagerFixture>
{
    [Fact]
    public async Task RegisterCustomer_Success()
    {
        var userModel = new CreateCustomerModel
        {
            Username = "TestUser",
            Email = "TestUser@gmail.com",
            Password = "Test123#",
            PhoneNumber = "123123123",
            StreetAddress = "Some street address",
            City = "Some city"
        };

        fixture
            .UserManagerMock.Setup(userManager =>
                userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())
            )
            .ReturnsAsync(IdentityResult.Success);

        var result = await fixture.CustomerService.RegisterCustomer(userModel);

        Assert.True(result.Succeeded);
        Assert.Equal("User created", result.Message);
        Assert.NotNull(result.Content);
    }

    [Fact]
    public async Task RegisterCustomer_Failure()
    {
        var userModel = new CreateCustomerModel
        {
            Username = "TestUser",
            Email = "TestUser@gmail.com",
            Password = "bad",
            PhoneNumber = "123123123",
            StreetAddress = "Some street address",
            City = "Some city",
        };

        fixture
            .UserManagerMock.Setup(userManager =>
                userManager.CreateAsync(It.IsAny<IdentityUser>(), userModel.Password)
            )
            .ReturnsAsync(IdentityResult.Failed());

        var result = await fixture.CustomerService.RegisterCustomer(userModel);

        Assert.False(result.Succeeded);
        Assert.Equal("User creation failed", result.Message);
        Assert.NotNull(result.Content);
    }
}

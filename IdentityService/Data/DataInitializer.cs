using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Data;

public class DataInitializer(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    public async Task SeedRoles()
    {
        // var roles = new[] { "Admin", "Customer" };
        //
        // foreach (var role in roles)
        // {
        //     if (!await roleManager.RoleExistsAsync(role))
        //     {
        //         await roleManager.CreateAsync(new IdentityRole(role));
        //     }
        // }
    }

    public async Task SeedUsers()
    {
        // var customer = new Customer
        // {
        //     UserName = "customer",
        //     NormalizedUserName = "customer".ToUpper(),
        //     Email = "customer@customer.se",
        //     NormalizedEmail = "customer@customer.se".ToUpper(),
        //     EmailConfirmed = true,
        //     PhoneNumber = "1234567890",
        //     PhoneNumberConfirmed = true,
        //     StreetAddress = "customeraddress1",
        //     City = "CustomerCity",
        //     DateOfBirth = new DateTime(2005 - 12 - 23)
        // };
        //
        // var admin = new Admin
        // {
        //     UserName = "admin",
        //     NormalizedUserName = "admin".ToUpper(),
        //     Email = "admin@admin.se",
        //     NormalizedEmail = "admin@admin.se".ToUpper(),
        //     EmailConfirmed = true,
        //     PhoneNumber = "1234567890",
        //     PhoneNumberConfirmed = true,
        //     AdminLevel = AdminLevel.SuperAdmin,
        // };
        //
        // await userManager.CreateAsync(customer, "Enter Password Here");
        // await userManager.CreateAsync(admin, "Enter Password Here");
        //
        // await userManager.AddToRoleAsync(customer, "Customer");
        // await userManager.AddToRoleAsync(admin, "Admin");
    }

    public async Task SeedUserRoles()
    {
        
    }
}
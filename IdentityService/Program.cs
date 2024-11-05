using Azure.Identity;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Version 1",
            Description = "Demo API with dummy data",
            Version = "v1"
        }
    );

    options.SwaggerDoc(
        "v2",
        new OpenApiInfo
        {
            Title = "Version 2",
            Description = "Version 2, now with real data!",
            Version = "v2"
        }
    );
});

// var connectionString = builder.Configuration.GetConnectionString("IdentityServiceConnectionString");

builder.Services.AddDbContext<DataContext>(o => o.UseMySQL(builder.Configuration["IdentityServiceConnectionString"]!));

var vaultUri = new Uri($"{builder.Configuration["KeyVault"]!}");
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        vaultUri,
        new VisualStudioCredential());
}
else
{
    builder.Configuration.AddAzureKeyVault(
        vaultUri,
        new DefaultAzureCredential());
}

builder.Services.AddDataProtection();

builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AdminService>();

// builder.Services.AddScoped<DataInitializer>();

builder
    .Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DataContext>();

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var customerManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
//
//     var dataInitializer = new DataInitializer(roleManager, customerManager);
//
//     await dataInitializer.SeedRoles();
//     await dataInitializer.SeedUsers();
//     await dataInitializer.SeedUserRoles();
// }

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1 (dummy)");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Version 2 (the real deal!)");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

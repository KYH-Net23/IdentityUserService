using Azure.Identity;
using IdentityService.Data;
using IdentityService.Infrastructure;
using IdentityService.Services;
using IdentityService.Services.HttpClientServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AuthorizationSettings>(
    builder.Configuration.GetSection("AuthorizationSettings")
);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpecificOrigin",
        policyBuilder =>
            policyBuilder
                .WithOrigins(
                    "http://localhost:5173",
                    "https://localhost:5173",
                    "https://localhost:7266"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
    );
});

builder.Services.AddDbContext<DataContext>(o =>
    o.UseMySQL(builder.Configuration["IdentityServiceConnectionString"]!)
);

var vaultUri = new Uri($"{builder.Configuration["KeyVault"]!}");

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(vaultUri, new VisualStudioCredential());
}
else
{
    builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
}

builder.Services.AddDataProtection();

builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddHttpClient<VerificationHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://rika-verification-provider.azurewebsites.net/api");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<EmailProviderHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://rika-solutions-email-provider.azurewebsites.net");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder
    .Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.SignIn.RequireConfirmedEmail = true;
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
//     // await dataInitializer.SeedRoles();
//     await dataInitializer.SeedUsers();
//     // await dataInitializer.SeedUserRoles();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();

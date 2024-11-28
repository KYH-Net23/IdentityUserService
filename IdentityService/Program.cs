using System;
using System.Text;
using Azure.Identity;
using IdentityService;
using IdentityService.Data;
using IdentityService.Infrastructure;
using IdentityService.Services;
using IdentityService.Services.HttpClientServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        }
    );
});
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
    client.BaseAddress = new Uri("https://rika-verification-provider.azurewebsites.net/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<AuthorizationHttpClient>(client =>
{
    client.BaseAddress = new Uri(""); // TODO token provider here
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

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["EmailProviderSecretKey"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true
        };
    });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

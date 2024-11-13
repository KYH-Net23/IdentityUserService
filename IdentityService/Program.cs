using Asp.Versioning;
using Azure.Identity;
using IdentityService.Data;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<AuthorizationSettings>(builder.Configuration.GetSection("AuthorizationSettings"));

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin",
		policyBuilder => policyBuilder.WithOrigins("http://localhost:5173", "https://localhost:5173","https://localhost:7266")
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(2, 0);
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ReportApiVersions = true;
});

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
builder.Services.AddScoped<UserService>();

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

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
	options.WithTheme(ScalarTheme.Mars)
		.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
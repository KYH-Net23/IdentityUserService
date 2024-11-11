using Azure.Identity;
using IdentityService.Data;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v2/swagger.json", "Version 2");
	c.SwaggerEndpoint("/swagger/v3/swagger.json", "Version 3");
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using IdentityService.Models.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext(options)
{
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Admin> Admins { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.Entity<Customer>()
			.ToTable("Customers");

		builder.Entity<Admin>()
			.ToTable("Admins");
	}
}
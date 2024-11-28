using IdentityService.Models.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext(options)
{
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<AdminEntity> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<CustomerEntity>().ToTable("Customers");

        builder.Entity<AdminEntity>().ToTable("Admins");
    }
}

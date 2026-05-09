using System;

// ScheduleApp.Infrastructure/Data/AppDbContext.cs
namespace ScheduleApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50);

            // Usuario de prueba con contraseña: Admin123*
            entity.HasData(new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                FullName = "Administrador",
                Email = "admin@scheduleapp.com",
                PasswordHash = "$2a$11$voq.ILCj3M49oj7lMN9l2.Tj9U56jcyrVUf12TxhFqlRydeKe99tS",
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // ✅ fijaa
            });
        });
    }
}

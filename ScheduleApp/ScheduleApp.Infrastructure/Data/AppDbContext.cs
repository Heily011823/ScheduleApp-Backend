using System;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Infrastructure.Data;

/// <summary>
/// DbContext principal de la aplicación.
/// Gestiona la conexión a la BD y el mapeo de entidades.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tabla de usuarios del sistema.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // PRIMARY KEY
            entity.HasKey(u => u.Id);

            // ÍNDICES ÚNICOS
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.IdentityDocument).IsUnique();

            // PROPIEDADES
            entity.Property(u => u.FullName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(u => u.Username)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(u => u.IdentityDocument)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.Property(u => u.Role)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(u => u.IsActive)
                  .IsRequired();

            entity.Property(u => u.CreatedAt)
                  .IsRequired();
        });
    }
}
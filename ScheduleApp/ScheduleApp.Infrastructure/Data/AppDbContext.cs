using System;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Infrastructure.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Materia> Materias { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.HasIndex(r => r.Name)
                  .IsUnique();
            entity.HasData(
                new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Administrador"
                },
                new Role
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Coordinador"
                }
            );
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.IdentityDocument).IsUnique();
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
            entity.Property(u => u.RoleId)
                  .IsRequired();
            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Property(u => u.IsActive)
                  .IsRequired();
            entity.Property(u => u.CreatedAt)
                  .IsRequired();
        });
        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.Codigo).IsUnique();
            entity.Property(m => m.Codigo)
                  .IsRequired()
                  .HasMaxLength(20);
            entity.Property(m => m.Nombre)
                  .IsRequired()
                  .HasMaxLength(150);
            entity.Property(m => m.Semestre)
                  .IsRequired();
            entity.Property(m => m.Creditos)
                  .IsRequired();
            entity.Property(m => m.HorasSemanales)
                  .IsRequired();
            entity.Property(m => m.IsActive)
                  .IsRequired();
            entity.Property(m => m.CreatedAt)
                  .IsRequired();
            entity.HasData(
                new Materia
                {
                    Id = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"),
                    Codigo = "MAT101",
                    Nombre = "Matematicas I",
                    Semestre = 1,
                    Creditos = 4,
                    HorasSemanales = 4,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Materia
                {
                    Id = Guid.Parse("aaaaaaaa-2222-2222-2222-222222222222"),
                    Codigo = "ING201",
                    Nombre = "Ingenieria de Software",
                    Semestre = 3,
                    Creditos = 3,
                    HorasSemanales = 4,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Materia
                {
                    Id = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"),
                    Codigo = "BD401",
                    Nombre = "Bases de Datos",
                    Semestre = 4,
                    Creditos = 4,
                    HorasSemanales = 6,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        });
    }
}
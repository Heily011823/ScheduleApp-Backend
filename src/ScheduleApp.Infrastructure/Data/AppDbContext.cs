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
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Assignment> Assignments => Set<Assignment>();

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

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");

            entity.HasKey(s => s.Id);

            entity.HasIndex(s => s.Code)
                .IsUnique();

            entity.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(s => s.Semester)
                .IsRequired();

            entity.Property(s => s.Credits)
                .IsRequired();

            entity.Property(s => s.WeeklyHours)
                .IsRequired();

            entity.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(s => s.CreatedAt)
                .IsRequired();

            entity.Property(s => s.UpdatedAt)
                .IsRequired(false);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Teacher)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Subject)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Classroom)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Day)
                .IsRequired();

            entity.Property(a => a.StartTime)
                .IsRequired();

            entity.Property(a => a.EndTime)
                .IsRequired();
        });
    }
}
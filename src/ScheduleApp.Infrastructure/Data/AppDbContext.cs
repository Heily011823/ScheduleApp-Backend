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
    public DbSet<TapsiRule> TapsiRules => Set<TapsiRule>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Program> Programs => Set<Program>();

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

            entity.Property(s => s.IsTapsi)
                .IsRequired()
                .HasDefaultValue(false);

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

        // Autor: Jacobo
        // Version: 0.1
        // Rama: feature/127-programas-crud-api
        modelBuilder.Entity<Program>(entity =>
        {
            entity.ToTable("Programs");

            entity.HasKey(p => p.Id);

            entity.HasIndex(p => p.Code)
                .IsUnique();

            entity.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.Property(p => p.UpdatedAt)
                .IsRequired(false);
        });


        /// Autor:  Mateo Quintero
        /// Version: 0.2
        /// rama: 33-Reglas-tapsi

        modelBuilder.Entity<TapsiRule>(entity =>
        {
            entity.ToTable("TapsiRules");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.RuleType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.UpdatedAt)
                .IsRequired(false);

            entity.HasData(
                new TapsiRule
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    RuleType = "MAX_DAILY_HOURS",
                    Description = "Las materias TAPSI no pueden superar 4 horas diarias.",
                    Value = "4",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new TapsiRule
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    RuleType = "ALLOWED_DAYS",
                    Description = "Las materias TAPSI solo pueden asignarse en días hábiles.",
                    Value = "Lunes,Martes,Miércoles,Jueves,Viernes",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new TapsiRule
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    RuleType = "TIME_RANGE",
                    Description = "Las materias TAPSI deben dictarse entre 7:00 y 18:00.",
                    Value = "07:00-18:00",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        });

        /// Autor:  Mateo Quintero
        /// Version: 0.2
        /// rama: 96-Crud-docentes
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(t => t.Id);

            entity.HasIndex(t => t.Email).IsUnique();
            entity.HasIndex(t => t.IdentityDocument).IsUnique();

            entity.Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.IdentityDocument)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20);

            entity.Property(t => t.Specialty)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.UpdatedAt)
                .IsRequired(false);
        });
    }
}

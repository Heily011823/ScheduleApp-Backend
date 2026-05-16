using Microsoft.EntityFrameworkCore;
using ScheduleApp.Domain.Entities;
using System;

// Ruta recomendada: src/ScheduleApp.Infrastructure/Data/AppDbContext.cs
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

    // NUEVOS DBSET PARA LA NORMALIZACIÓN
    public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==========================================
        // CONFIGURACIÓN: ROLE
        // ==========================================
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

        // ==========================================
        // CONFIGURACIÓN: USER
        // ==========================================
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

        // ==========================================
        // CONFIGURACIÓN: SUBJECT
        // ==========================================
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

        // ==========================================
        // CONFIGURACIÓN: ASSIGNMENT
        // ==========================================
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

        // ==========================================
        // CONFIGURACIÓN: TAPSI RULES
        // ==========================================
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

        // ==========================================
        // CONFIGURACIÓN: TEACHERS (¡Completamente Normalizado!)
        // ==========================================
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(t => t.Id);

            entity.HasIndex(t => t.Email).IsUnique();
            entity.HasIndex(t => t.IdentityDocument).IsUnique();

            entity.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.IdentityDocument)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20);

            // SE ELIMINARON: AcademicProgram, PreferredSubject y Availability de aquí.
            // Ahora la información vive de forma estructurada en sus respectivas tablas relaciones.

            entity.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.UpdatedAt)
                .IsRequired(false);
        });

        // ==========================================
        // CONFIGURACIÓN: TEACHER AVAILABILITY (Relación 1 a Muchos)
        // ==========================================
        modelBuilder.Entity<TeacherAvailability>(entity =>
        {
            entity.ToTable("TeacherAvailabilities");
            entity.HasKey(ta => ta.Id);

            entity.Property(ta => ta.Day)
                .IsRequired();

            entity.Property(ta => ta.StartTime)
                .IsRequired();

            entity.Property(ta => ta.EndTime)
                .IsRequired();

            entity.Property(ta => ta.MaxTeachingHours)
                .IsRequired();

            // Configuración de la llave foránea con borrado en cascada
            entity.HasOne(ta => ta.Teacher)
                .WithMany(t => t.Availabilities)
                .HasForeignKey(ta => ta.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ==========================================
        // CONFIGURACIÓN: TEACHER SUBJECT (Relación Muchos a Muchos Intermedia)
        // ==========================================
        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.ToTable("TeacherSubjects");

            // Llave primaria compuesta reglamentaria
            entity.HasKey(ts => new { ts.TeacherId, ts.SubjectId });

            entity.Property(ts => ts.ContractType)
                .IsRequired()
                .HasMaxLength(50);

            // Relación inversa hacia Teacher
            entity.HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación inversa hacia la tabla global existente de Subjects
            entity.HasOne(ts => ts.Subject)
                .WithMany()
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
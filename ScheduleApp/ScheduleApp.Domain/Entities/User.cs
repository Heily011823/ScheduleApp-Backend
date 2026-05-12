using System;

namespace ScheduleApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string IdentityDocument { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    // Foreign Key
    public Guid RoleId { get; set; }

    // Navegación
    public Role Role { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
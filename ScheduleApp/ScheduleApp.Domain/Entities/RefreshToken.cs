using System;

// ScheduleApp.Domain/Entities/RefreshToken.cs
namespace ScheduleApp.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;

    // Foreign Key
    public Guid UserId { get; set; }
    // Navegación
    public User User { get; set; } = null!;

    // Propiedades calculadas
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}

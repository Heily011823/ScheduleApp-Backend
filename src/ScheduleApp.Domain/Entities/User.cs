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

    // Llave Foránea y Navegación a Roles
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // Estado funcional: puede variar entre activo e inactivo.
    public bool IsActive { get; set; } = true;

    // Eliminacion logica: una vez true, el usuario queda fuera del sistema permanentemente.
    // No debe revertirse. Lo distingue del estado IsActive que si puede cambiar.
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

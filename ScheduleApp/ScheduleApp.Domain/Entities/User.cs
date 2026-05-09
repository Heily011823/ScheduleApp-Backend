using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad principal que representa a un usuario del sistema.
/// Contiene la información de autenticación y perfil básico.
/// </summary>
/// Autor:  Mateo Quintero 
/// Version: 0.1
public class User
{
    /// <summary>Identificador único del usuario (GUID).</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre completo del usuario.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Correo electrónico. Se usa como identificador de login. Debe ser único.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash de la contraseña generado con BCrypt. Nunca almacenar texto plano.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Rol del usuario en el sistema. Ejemplo: "Admin", "Teacher".</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Indica si el usuario está activo. Los inactivos no pueden autenticarse.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Fecha de creación del registro en UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
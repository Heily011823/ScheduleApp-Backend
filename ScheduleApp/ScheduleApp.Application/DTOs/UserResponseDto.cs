namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO de salida que representa a un usuario en las respuestas del API.
/// NUNCA expone el PasswordHash.
/// </summary>
/// Autor: Jacobo
/// Version: 0.2
public class UserResponseDto
{
    /// <summary>Identificador único del usuario.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre completo del usuario.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Correo electrónico del usuario.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Nombre de usuario.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Documento de identidad.</summary>
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Rol del usuario en el sistema.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Indica si el usuario está activo.</summary>
    public bool IsActive { get; set; }

    /// <summary>Fecha de creación del registro.</summary>
    public DateTime CreatedAt { get; set; }
}
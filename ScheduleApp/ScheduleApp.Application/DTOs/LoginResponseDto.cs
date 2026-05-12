using System;

// ScheduleApp.Application/DTOs/LoginResponseDto.cs
// DTO de salida del endpoint POST /api/auth/login.
// Contiene el token JWT y datos básicos del usuario autenticado.

/// Autor:  Mateo Quintero 
/// Version: 0.1

namespace ScheduleApp.Application.DTOs;

public class LoginResponseDto
{
    /// <summary>Token JWT firmado. El cliente debe enviarlo en el header Authorization: Bearer {token}.</summary>
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Tipo de token. Siempre "Bearer" para JWT.</summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>Fecha y hora de expiración del token en UTC. Por defecto 60 minutos desde la emisión.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Nombre completo del usuario autenticado. Útil para mostrar en la UI.</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>Rol del usuario. Ejemplo: "Admin", "Teacher". Útil para control de acceso en el frontend.</summary>
    public string Role { get; set; } = string.Empty;
}
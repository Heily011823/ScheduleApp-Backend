using System;

// ScheduleApp.Application/DTOs/LoginRequestDto.cs
// DTO de entrada para el endpoint POST /api/auth/login.
// Contiene las credenciales que el usuario envía para autenticarse.

/// Autor:  Mateo Quintero 
/// Version: 0.1

namespace ScheduleApp.Application.DTOs;

public class LoginRequestDto
{
    /// <summary>Correo electrónico del usuario. Se usa como identificador único de login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña en texto plano. Se compara contra el hash almacenado en BD.</summary>
    public string Password { get; set; } = string.Empty;
}
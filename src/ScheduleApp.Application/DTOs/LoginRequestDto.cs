using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO de entrada para el endpoint POST /api/auth/login.
/// Permite autenticarse con correo institucional o nombre de usuario.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.2
public class LoginRequestDto
{
    /// <summary>
    /// Correo institucional o nombre de usuario.

    /// </summary>
    [Required(ErrorMessage = "El usuario o correo es obligatorio")]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña en texto plano. Se compara contra el hash almacenado en BD.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string Password { get; set; } = string.Empty;
}
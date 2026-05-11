using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO de entrada para crear un nuevo usuario.
/// Contiene los datos que el cliente envía en el endpoint POST.
/// La contraseña se recibe en texto plano y se hashea en el service antes de persistir.
/// </summary>
/// Autor: Jacobo
/// Version: 0.1
public class CreateUserDto
{
    /// <summary>Nombre completo del usuario.</summary>
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Correo electrónico único. Se usará como identificador de login.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña en texto plano. Será hasheada con BCrypt en el service.</summary>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Rol del usuario (ej. "Admin", "Teacher", "Coordinator").</summary>
    [Required(ErrorMessage = "El rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres")]
    public string Role { get; set; } = string.Empty;
}
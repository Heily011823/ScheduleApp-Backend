using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO de entrada para actualizar un usuario existente.
/// </summary>
public class UpdateUserDto
{
    /// <summary>Nombre completo del usuario.</summary>
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Correo electrónico único.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Nombre de usuario.</summary>
    [Required(ErrorMessage = "El usuario es obligatorio")]
    [StringLength(50, ErrorMessage = "El usuario no puede exceder los 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    /// <summary>Documento de identidad.</summary>
    [Required(ErrorMessage = "El documento es obligatorio")]
    [StringLength(20, ErrorMessage = "El documento no puede exceder los 20 caracteres")]
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Contraseña opcional para actualización.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Rol del usuario.</summary>
    [Required(ErrorMessage = "El rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres")]
    public string Role { get; set; } = string.Empty;

    /// <summary>Permite activar o desactivar manualmente al usuario.</summary>
    public bool IsActive { get; set; } = true;
}
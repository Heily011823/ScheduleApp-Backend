using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO de entrada para actualizar un usuario existente.
/// No incluye contraseña por seguridad; el cambio de contraseña debería ser
/// un endpoint dedicado en el futuro.
/// </summary>
/// Autor: Jacobo
/// Version: 0.1
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

    /// <summary>Rol del usuario.</summary>
    [Required(ErrorMessage = "El rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres")]
    public string Role { get; set; } = string.Empty;

    /// <summary>Permite activar o desactivar manualmente al usuario.</summary>
    public bool IsActive { get; set; } = true;
}
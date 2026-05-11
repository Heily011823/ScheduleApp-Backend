using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO de entrada para crear un nuevo usuario.
/// Contiene los datos que el cliente envía en el endpoint POST.
/// La contraseña se recibe en texto plano y se hashea en el service antes de persistir.
/// </summary>
/// Autor: Jacobo
/// Version: 0.2
public class CreateUserDto
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

    /// <summary>Nombre de usuario para iniciar sesión.</summary>
    [Required(ErrorMessage = "El usuario es obligatorio")]
    [StringLength(50, ErrorMessage = "El usuario no puede exceder los 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    /// <summary>Documento de identidad del usuario.</summary>
    [Required(ErrorMessage = "El documento de identidad es obligatorio")]
    [StringLength(20, ErrorMessage = "El documento no puede exceder los 20 caracteres")]
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Contraseña en texto plano.</summary>
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Confirmación de contraseña.</summary>
    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>Rol del usuario.</summary>
    [Required(ErrorMessage = "El rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres")]
    public string Role { get; set; } = string.Empty;

    /// <summary>Estado del usuario. Ej: Activo o Inactivo.</summary>
    [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres")]
    public string Status { get; set; } = "Activo";
}
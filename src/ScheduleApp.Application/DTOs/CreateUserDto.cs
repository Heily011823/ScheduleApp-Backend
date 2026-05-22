using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
    [StringLength(150)]
    [RegularExpression(
        @"^[a-zA-Z0-9._%+\-]+@autonoma\.edu\.co$",
        ErrorMessage = "El email debe pertenecer al dominio @autonoma.edu.co")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El usuario es obligatorio")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es obligatorio")]
    [StringLength(20)]
    public string IdentityDocument { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio")]
    public string Status { get; set; } = "Activo";
}
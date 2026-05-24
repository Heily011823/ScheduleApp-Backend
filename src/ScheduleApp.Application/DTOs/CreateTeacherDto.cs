using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

// Ruta recomendada: src/ScheduleApp.Application/DTOs/CreateTeacherDto.cs
namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO para registrar un nuevo docente en el sistema.
/// Contiene la información básica requerida por el coordinador.
/// </summary>
public class CreateTeacherDto
{
    /// <summary>Nombres del docente.</summary>
    /// 
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apellidos del docente.</summary>
    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Correo electrónico institucional único.</summary>
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Documento de identidad único.</summary>
    [Required(ErrorMessage = "El documento es requerido")]
    [StringLength(20, MinimumLength = 5)]
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Número telefónico de contacto.</summary>
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Lista de IDs de especialidades asignadas al docente
    /// </summary>
    public List<Guid> SpecialtyIds { get; set; } = new List<Guid>();

    /// <summary>
    /// Cantidad de horas disponibles para dictar clases.
    /// </summary>
    [Range(0, 40, ErrorMessage = "Las horas deben estar entre 0 y 40")]
    public int TeachingHours { get; set; }

    /// <summary>
    /// Tipo de contrato del docente.
    /// Ejemplo: Tiempo completo, Cátedra.
    /// </summary>
    public string ContractType { get; set; } = "No asignado";

   


}
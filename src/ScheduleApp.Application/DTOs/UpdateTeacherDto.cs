using System;
using System.Collections.Generic;
using System.Text;


namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO para actualizar la información de un docente existente.
/// </summary>
public class UpdateTeacherDto
{
    /// <summary>Nombres del docente.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apellidos del docente.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Correo electrónico institucional.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Documento de identidad.</summary>
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Teléfono de contacto.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Especialidades del docente.
    /// Ejemplo: Matemáticas, Programación.
    /// </summary>
    public string Specialties { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de horas asignadas para dictar clases.
    /// </summary>
    public int TeachingHours { get; set; }

    /// <summary>
    /// Tipo de contrato del docente.
    /// </summary>
    public string ContractType { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el docente está activo o inactivo.
    /// </summary>
    public bool IsActive { get; set; }
}
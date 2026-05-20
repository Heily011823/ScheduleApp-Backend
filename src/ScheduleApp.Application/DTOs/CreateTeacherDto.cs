using System;
using System.Collections.Generic;
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
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apellidos del docente.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Correo electrónico institucional único.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Documento de identidad único.</summary>
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Número telefónico de contacto.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Especialidades del docente.
    /// Ejemplo: Matemáticas, Programación.
    /// </summary>
    public string Specialties { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de horas disponibles para dictar clases.
    /// </summary>
    public int TeachingHours { get; set; }

    /// <summary>
    /// Tipo de contrato del docente.
    /// Ejemplo: Tiempo completo, Cátedra.
    /// </summary>
    public string ContractType { get; set; } = string.Empty;
}
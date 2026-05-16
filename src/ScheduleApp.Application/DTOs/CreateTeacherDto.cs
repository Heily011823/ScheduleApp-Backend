using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/DTOs/CreateTeacherDto.cs
namespace ScheduleApp.Application.DTOs;


/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes

/// <summary>
/// DTO para registrar un nuevo docente en el sistema.
/// Todos los campos son requeridos excepto PhoneNumber.
/// </summary>
public class CreateTeacherDto
{
    /// <summary>Nombre completo del docente.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Correo electrónico institucional único.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Número de documento de identidad único.</summary>
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Número de teléfono de contacto (opcional).</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Especialidad académica. Ej: "Matemáticas".</summary>
    public string Specialty { get; set; } = string.Empty;
}

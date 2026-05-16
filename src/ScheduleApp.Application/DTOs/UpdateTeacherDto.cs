using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/DTOs/UpdateTeacherDto.cs
namespace ScheduleApp.Application.DTOs;


/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes

/// <summary>
/// DTO para actualizar los datos de un docente existente.
/// </summary>
public class UpdateTeacherDto
{
    /// <summary>Nuevo nombre completo del docente.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Nuevo correo electrónico.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Nuevo número de documento.</summary>
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Nuevo teléfono de contacto.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Nueva especialidad.</summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>Estado activo o inactivo del docente.</summary>
    public bool IsActive { get; set; }
}

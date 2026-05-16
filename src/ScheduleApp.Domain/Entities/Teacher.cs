

// src/ScheduleApp.Domain/Entities/Teacher.cs
using System;
using System.Collections.Generic;
using System.Text;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa un docente del sistema académico.
/// Los docentes son gestionados por el coordinador y se asignan
/// a materias en la generación de horarios.
/// No son usuarios del sistema — no tienen credenciales de acceso.
/// </summary>

public class Teacher
{
    /// <summary>Identificador único del docente (GUID).</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre completo del docente.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Correo electrónico institucional. Debe ser único.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Número de documento de identidad. Debe ser único.</summary>
    public string IdentityDocument { get; set; } = string.Empty;

    /// <summary>Número de teléfono de contacto.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Especialidad académica del docente.
    /// Ej: "Matemáticas", "Ingeniería de Software", "Física".
    /// Usado por rama 41 para filtrar disponibilidad por especialidad.
    /// </summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el docente está activo en el sistema.
    /// Los docentes inactivos no pueden ser asignados a horarios.
    /// Usado por rama 42 para validar cruces de horario.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Fecha de registro del docente en UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de última modificación en UTC. Null si nunca fue editado.</summary>
    public DateTime? UpdatedAt { get; set; }
}

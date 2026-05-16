
// ScheduleApp.Domain/Entities/Subject.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa una materia del plan de estudios.
/// Las materias marcadas como TAPSI tienen restricciones especiales
/// que se aplican durante la generación de horarios.
/// </summary>
public class Subject
{
    public Guid Id { get; set; }

    /// <summary>Código único de la materia. Ej: "MAT101".</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Nombre completo de la materia.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Semestre al que pertenece la materia (1-10).</summary>
    public int Semester { get; set; }

    /// <summary>Número de créditos académicos.</summary>
    public int Credits { get; set; }

    /// <summary>Horas semanales de clase.</summary>
    public int WeeklyHours { get; set; }

    /// <summary>
    /// Indica si la materia pertenece al programa TAPSI.
    /// Las materias TAPSI tienen reglas especiales de horario.
    /// </summary>
    public bool IsTapsi { get; set; } = false;

    /// <summary>Indica si la materia está activa en el sistema.</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
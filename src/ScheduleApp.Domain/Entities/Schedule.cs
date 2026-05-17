using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad central que representa la asignación de una clase en un espacio y tiempo determinados.
/// </summary>
public class Schedule
{
    public Guid Id { get; set; }

    // 1. RELACIONES (Claves Foráneas)
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public Guid ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }


    // 2. DATOS PROPIOS DEL HORARIO (Según Criterios de Aceptación)
    public DayOfWeek Day { get; set; }          // Día (Lunes, Martes, etc.)
    public TimeSpan StartTime { get; set; }     // Hora inicio (ej: 07:00:00)
    public TimeSpan EndTime { get; set; }       // Hora fin (ej: 09:00:00)

    public string AcademicProgram { get; set; } = string.Empty; // Programa (ej: Ingeniería de sistemas)
    public string Shift { get; set; } = string.Empty;           // Jornada (ej: Diurna, Nocturna)
    public int Semester { get; set; }                           // Semestre (ej: 3)
    public string Status { get; set; } = "Draft";               // Estado (ej: Draft, Confirmed)

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
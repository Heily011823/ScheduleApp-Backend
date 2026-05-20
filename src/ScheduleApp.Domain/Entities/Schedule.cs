using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad central que representa la asignación de una clase en un espacio y tiempo determinados.
/// </summary>
public class Schedule
{
    public Guid Id { get; set; }

    // Relaciones (Claves Foráneas)
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public Guid TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public Guid ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }

    // Datos propios del horario asignado
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public string AcademicProgram { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string Status { get; set; } = "Draft";               // Draft, Confirmed

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
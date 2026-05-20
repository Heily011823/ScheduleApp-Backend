using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa una materia del plan de estudios.
/// </summary>
public class Subject
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Semester { get; set; }
    public int Credits { get; set; }
    public int WeeklyHours { get; set; }

    // Regla especial TAPSI
    public bool IsTapsi { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
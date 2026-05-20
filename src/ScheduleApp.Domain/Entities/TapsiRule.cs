using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa una regla especial aplicable a materias TAPSI durante la generación de horarios.
/// </summary>
public class TapsiRule
{
    public Guid Id { get; set; }
    public string RuleType { get; set; } = string.Empty; // MAX_DAILY_HOURS, ALLOWED_DAYS, etc.
    public string Description { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
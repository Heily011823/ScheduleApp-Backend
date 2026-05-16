
// ScheduleApp.Domain/Entities/TapsiRule.cs

// ScheduleApp.Domain/Entities/TapsiRule.cs
using System;
using System.Collections.Generic;
using System.Text;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi


namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa una regla especial aplicable a materias TAPSI
/// durante la generación de horarios.
///
/// Tipos de reglas soportados:
/// - "MAX_DAILY_HOURS"    → Máximo de horas diarias permitidas
/// - "ALLOWED_DAYS"       → Días de la semana permitidos (ej: "Lunes,Miércoles")
/// - "TIME_RANGE"         → Rango horario permitido (ej: "07:00-12:00")
/// - "MAX_WEEKLY_HOURS"   → Máximo de horas semanales
/// </summary>
public class TapsiRule
{
    /// <summary>Identificador único de la regla.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tipo de regla. Define qué restricción aplica.
    /// Valores posibles: MAX_DAILY_HOURS, ALLOWED_DAYS, TIME_RANGE, MAX_WEEKLY_HOURS.
    /// </summary>
    public string RuleType { get; set; } = string.Empty;

    /// <summary>
    /// Descripción legible de la regla para mostrar al coordinador.
    /// Ej: "Las materias TAPSI no pueden superar 4 horas diarias".
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Valor de la regla en formato string.
    /// Ej: "4" para MAX_DAILY_HOURS, "Lunes,Miércoles,Viernes" para ALLOWED_DAYS.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Indica si la regla está activa y debe aplicarse.</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
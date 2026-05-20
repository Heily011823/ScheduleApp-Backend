using System;
using System.Collections.Generic;
using System.Text;

// ScheduleApp.Application/DTOs/TapsiRuleDto.cs
namespace ScheduleApp.Application.DTOs;


/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi


/// <summary>DTO para crear o actualizar una regla TAPSI.</summary>
public class TapsiRuleDto
{
    /// <summary>Tipo de regla. Ej: "MAX_DAILY_HOURS".</summary>
    public string RuleType { get; set; } = string.Empty;

    /// <summary>Descripción legible de la regla.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Valor de la regla. Ej: "4".</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Indica si la regla está activa.</summary>
    public bool IsActive { get; set; } = true;
}

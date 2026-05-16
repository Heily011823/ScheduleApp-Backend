using System;
using System.Collections.Generic;
using System.Text;

// ScheduleApp.Application/DTOs/TapsiValidationRequestDto.cs
namespace ScheduleApp.Application.DTOs;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi

/// <summary>
/// DTO con los datos de una asignación para validar contra reglas TAPSI.
/// </summary>

public class TapsiValidationRequestDto
{
    /// <summary>ID de la materia a validar.</summary>
    public Guid SubjectId { get; set; }

    /// <summary>Día de la semana. Ej: "Lunes".</summary>
    public string Day { get; set; } = string.Empty;

    /// <summary>Hora de inicio. Ej: "07:00".</summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>Hora de fin. Ej: "11:00".</summary>
    public string EndTime { get; set; } = string.Empty;
}
using System;
using System.Collections.Generic;
using System.Text;

// ScheduleApp.Application/DTOs/TapsiRuleResponseDto.cs
namespace ScheduleApp.Application.DTOs;


/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi


/// <summary>DTO de respuesta para una regla TAPSI.</summary>
public class TapsiRuleResponseDto
{
    public Guid Id { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

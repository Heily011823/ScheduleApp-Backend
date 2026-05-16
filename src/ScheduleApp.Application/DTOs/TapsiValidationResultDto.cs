using System;
using System.Collections.Generic;
using System.Text;

// ScheduleApp.Application/DTOs/TapsiValidationResultDto.cs
namespace ScheduleApp.Application.DTOs;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi



/// <summary>
/// Resultado de validar una asignación contra las reglas TAPSI.
/// Contiene la lista de advertencias encontradas.
/// </summary>
public class TapsiValidationResultDto
{
    /// <summary>True si la asignación cumple todas las reglas TAPSI.</summary>
    public bool IsValid { get; set; }

    /// <summary>Lista de advertencias cuando alguna regla no se cumple.</summary>
    public List<string> Warnings { get; set; } = new();
}
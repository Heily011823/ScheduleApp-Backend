using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/DTOs/ChangeStatusDto.cs
namespace ScheduleApp.Application.DTOs;

/// <summary>
/// DTO para cambiar el estado activo/inactivo de un docente.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
/// Rama: 99-implementar-cambio-de-estado-de-docente
public class ChangeStatusDto
{
    /// <summary>
    /// true = activar docente, false = desactivar docente.
    /// </summary>
    public bool IsActive { get; set; }
}

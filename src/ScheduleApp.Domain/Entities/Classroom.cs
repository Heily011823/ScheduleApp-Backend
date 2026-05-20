// src/ScheduleApp.Domain/Entities/Classroom.cs
using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un espacio académico (Aula) usando identificadores únicos globales.
/// </summary>
public class Classroom
{
    // Identificador único de la entidad (Guid)
    public Guid Id { get; set; }

    // Código académico único del aula (ej: 102001)
    public string Code { get; set; } = string.Empty;

    // Nombre específico (ej: F-401 o Bloque 16-103)
    public string Name { get; set; } = string.Empty;

    // Ubicación (ej: Edificio Fundadores, Edificio Sacatín)
    public string Building { get; set; } = string.Empty;

    // Número de piso donde se encuentra
    public int Floor { get; set; }

    // Capacidad máxima de estudiantes
    public int Capacity { get; set; }

    // Tipo de mobiliario/aula (ej: Laboratorio, Sillas universitarias)
    public string Type { get; set; } = string.Empty;

    // Control de estado lógico para el Criterio #85
    public bool IsActive { get; set; } = true;

    // Campos de auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
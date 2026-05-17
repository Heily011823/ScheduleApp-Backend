using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un Programa Académico de la institución.
/// </summary>
public class AcademicProgram
{
    public Guid Id { get; set; }

    // Código único del programa (ej: "1030", "1023")
    public string Code { get; set; } = string.Empty;

    // Nombre (ej: "Ingeniería de Sistemas")
    public string Name { get; set; } = string.Empty;

    // Jornada (ej: "Diurna", "Nocturna")
    public string Shift { get; set; } = string.Empty;

    // Cantidad de semestres que dura la carrera (ej: 10)
    public int TotalSemesters { get; set; }

    // Estado lógico (Activa / Inactiva)
    public bool IsActive { get; set; } = true;

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
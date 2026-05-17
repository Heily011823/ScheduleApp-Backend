using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un espacio académico (Aula).
/// </summary>
public class Classroom
{
    // Usamos Guid para mantener la consistencia arquitectónica del backend
    public Guid Id { get; set; }

    // Código académico único del aula (ej: 102001)
    public string Code { get; set; } = string.Empty;

    // Nombre específico (ej: F-401 o Bloque 16-103)
    public string Name { get; set; } = string.Empty;

    // Ubicación (ej: Edificio Fundadores, Edificio Sacatín)
    public string Building { get; set; } = string.Empty;

    // Número de piso donde se encuentra
    public int Floor { get; set; }

    // Capacidad máxima de estudiantes (ej: 3, 4 o más)
    public int Capacity { get; set; }

    // Tipo de mobiliario/aula (ej: Sillas giratorias, Sillas universitarias, Laboratorio)
    public string Type { get; set; } = string.Empty;

    // Control de estado lógico (Activa / Inactiva)
    public bool IsActive { get; set; } = true;

    // Auditoría básica obligatoria
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
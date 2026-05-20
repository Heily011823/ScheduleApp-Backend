using System;

namespace ScheduleApp.Domain.Entities;

// Entidad de dominio que representa un Programa Academico de la institucion.
public class AcademicProgram
{
    public Guid Id { get; set; }

    // Codigo unico del programa (ej: "1030", "1023")
    public string Code { get; set; } = string.Empty;

    // Nombre (ej: "Ingenieria de Sistemas")
    public string Name { get; set; } = string.Empty;

    // Jornada (ej: "Diurna", "Nocturna")
    public string Shift { get; set; } = string.Empty;

    // Cantidad de semestres que dura la carrera (ej: 10)
    public int TotalSemesters { get; set; }

    // Estado funcional: puede variar entre activo e inactivo (ej: se oferta este semestre o no).
    // Lo controla el coordinador, no debe usarse para "eliminacion".
    public bool IsActive { get; set; } = true;

    // Eliminacion logica: una vez true, el registro queda fuera del sistema permanentemente.
    // No debe revertirse. Lo distingue del estado IsActive que si puede cambiar.
    public bool IsDeleted { get; set; } = false;

    // Auditoria
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

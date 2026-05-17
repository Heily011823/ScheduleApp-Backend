using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa la configuración específica de un semestre para un programa académico.
/// </summary>
public class ProgramSemester
{
    public Guid Id { get; set; }

    // Relación con el Programa Académico (Clave Foránea)
    public Guid AcademicProgramId { get; set; }
    public AcademicProgram? AcademicProgram { get; set; }

    // Número del semestre (ej: 1, 2, 3, 5...)
    public int SemesterNumber { get; set; }

    // Carga de créditos sugerida/máxima para este semestre (ej: 15, 18)
    public int MaxCredits { get; set; }

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
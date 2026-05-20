using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Tabla intermedia para la relación Muchos a Muchos entre Teachers y Subjects.
/// </summary>
public class TeacherSubject
{
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    // Atributo adicional de la relación
    public string ContractType { get; set; } = string.Empty;
}
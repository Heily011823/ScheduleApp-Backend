// src/ScheduleApp.Domain/Entities/ClassroomAssignment.cs
using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa una asignación de aula en un horario específico.
/// Usada por el servicio de disponibilidad de aulas.
/// </summary>
public class ClassroomAssignment
{
    // Identificador único global de la asignación
    public Guid Id { get; set; }

    // Identificador único de la relación con el aula (Guid)
    public Guid ClassroomId { get; set; }
    public Classroom Classroom { get; set; } = null!;

    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
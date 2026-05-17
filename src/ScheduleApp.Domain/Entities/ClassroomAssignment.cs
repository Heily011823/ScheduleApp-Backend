using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Representa una asignación de aula en un horario específico.
/// Usada por el servicio de disponibilidad de aulas.
/// </summary>
public class ClassroomAssignment
{
    public int Id { get; set; }

   
    public Guid ClassroomId { get; set; }
    public Classroom Classroom { get; set; } = null!;

    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
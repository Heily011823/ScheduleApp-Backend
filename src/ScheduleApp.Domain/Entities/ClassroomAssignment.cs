// src/ScheduleApp.Domain/Entities/ClassroomAssignment.cs
namespace ScheduleApp.Domain.Entities;

/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: develop

/// <summary>
/// Representa una asignación de aula en un horario específico.
/// Usada por el servicio de disponibilidad de aulas.
/// </summary>
public class ClassroomAssignment
{
    public int Id { get; set; }
    public int ClassroomId { get; set; }
    public Classroom Classroom { get; set; } = null!;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
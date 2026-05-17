using System;
<<<<<<< HEAD

namespace ScheduleApp.Domain.Entities;

=======

// src/ScheduleApp.Domain/Entities/ClassroomAssignment.cs
namespace ScheduleApp.Domain.Entities;

/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: develop
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
/// <summary>
/// Representa una asignación de aula en un horario específico.
/// Usada por el servicio de disponibilidad de aulas.
/// </summary>
public class ClassroomAssignment
{
<<<<<<< HEAD
    public int Id { get; set; }

   
    public Guid ClassroomId { get; set; }
=======
    // CAMBIA AQUÍ: De int a Guid para consistencia arquitectónica
    public Guid Id { get; set; }

    // CAMBIA AQUÍ: De int a Guid para que coincida con el nuevo Id de Classroom
    public Guid ClassroomId { get; set; }

>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
    public Classroom Classroom { get; set; } = null!;

    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
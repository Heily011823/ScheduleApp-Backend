using System;
using System.Collections.Generic;

namespace ScheduleApp.Domain.Entities;

public class Teacher
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentityDocument { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relación 1 a Muchos con Disponibilidad
    public ICollection<TeacherAvailability> Availabilities { get; set; } = new List<TeacherAvailability>();

    // Relación Muchos a Muchos con Materias (Tabla intermedia)
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

}
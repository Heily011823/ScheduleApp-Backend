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



    [Obsolete("Use TeacherSpecialties instead")]
    public Guid? SpecialtyId { get; set; }

    [Obsolete("Use TeacherSpecialties instead")]
    public Specialty? Specialty { get; set; }

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    // relación muchos a muchos con Especialidades

    public ICollection<TeacherSpecialty> TeacherSpecialties { get; set; } = new List<TeacherSpecialty>();

    // Relación 1 a Muchos con Disponibilidad
    public ICollection<TeacherAvailability> Availabilities { get; set; } = new List<TeacherAvailability>();

    // Relación Muchos a Muchos con Materias (Tabla intermedia)
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();


}
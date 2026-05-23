using System;

namespace ScheduleApp.Domain.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Semester { get; set; }
    public int Credits { get; set; }
    public int WeeklyHours { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTapsi { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }



    // ✅ Estas dos líneas DEBEN estar UNA SOLA VEZ
    public Guid? SpecialtyId { get; set; }
    public Specialty? Specialty { get; set; }

    // Relación existente (si la tienes)
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}
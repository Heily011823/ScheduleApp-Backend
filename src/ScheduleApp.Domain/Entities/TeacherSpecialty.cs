using System;
using System.Collections.Generic;
using System.Text;
using System;

namespace ScheduleApp.Domain.Entities;

/// <summary>
/// Entidad intermedia para la relación muchos a muchos entre Teacher y Specialty
/// </summary>
public class TeacherSpecialty
{
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public Guid SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}

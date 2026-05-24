using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Domain.Entities
{
    public class Specialty
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relación con materias (1 a muchos)
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();

        // ✅ NUEVO: Relación con docentes (muchos a muchos a través de TeacherSpecialty)
        public ICollection<TeacherSpecialty> TeacherSpecialties { get; set; } = new List<TeacherSpecialty>();
    }
}


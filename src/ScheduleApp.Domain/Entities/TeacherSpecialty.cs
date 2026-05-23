using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Domain.Entities
{
    internal class TeacherSpecialty
    {

        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public Guid SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}

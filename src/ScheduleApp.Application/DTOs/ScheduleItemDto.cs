// Autor: Jacobo
// Version: 0.1

using System;

namespace ScheduleApp.Application.DTOs
{
    // Representa una materia disponible para ser usada
    // durante el proceso de generación del horario.
    public class ScheduleItemDto
    {
        public Guid SubjectId { get; set; }

        public string SubjectCode { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public int Semester { get; set; }

        public int Credits { get; set; }

        public int WeeklyHours { get; set; }

        // true = obligatoria / TAPSI
        // false = opcional / no TAPSI
        public bool IsTapsi { get; set; }

        public Guid TeacherId { get; set; }

        public string TeacherFullName { get; set; } = string.Empty;

        public string ContractType { get; set; } = string.Empty;
    }
}
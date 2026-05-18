// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Application.DTOs
{
    // Representa una clase ya asignada dentro del horario generado.
    // Se incluyen nombres y codigos legibles para que el frontend
    // no tenga que hacer joins adicionales.
    public class GeneratedScheduleEntryDto
    {
        public Guid Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
        public string ClassroomCode { get; set; } = string.Empty;
        public string ClassroomName { get; set; } = string.Empty;
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

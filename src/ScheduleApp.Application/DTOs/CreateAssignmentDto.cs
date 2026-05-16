// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Application.DTOs
{
    // DTO de entrada para crear un nuevo horario (Assignment).
    // No incluye Id porque la base de datos lo genera automaticamente.
    public class CreateAssignmentDto
    {
        public string Teacher { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

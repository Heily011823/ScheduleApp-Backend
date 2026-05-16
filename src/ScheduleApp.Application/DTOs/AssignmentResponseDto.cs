// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Application.DTOs
{
    // DTO de salida con los datos completos de un horario, incluyendo su Id.
    // Se usa para no exponer la entidad de dominio directamente en las respuestas HTTP.
    public class AssignmentResponseDto
    {
        public int Id { get; set; }
        public string Teacher { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

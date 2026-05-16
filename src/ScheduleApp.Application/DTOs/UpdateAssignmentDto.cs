// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Application.DTOs
{
    // DTO de entrada para actualizar un horario existente.
    // Tiene los mismos campos que CreateAssignmentDto pero se mantiene separado
    // para permitir reglas de validacion distintas a futuro (ej. campos opcionales).
    public class UpdateAssignmentDto
    {
        public string Teacher { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

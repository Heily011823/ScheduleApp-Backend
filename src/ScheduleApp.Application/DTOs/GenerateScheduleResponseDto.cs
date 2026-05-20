// Autor: Jacobo
// Version: 0.1

using System.Collections.Generic;

namespace ScheduleApp.Application.DTOs
{
    // Respuesta del generador de horarios.
    // Success indica si al menos una clase pudo ser programada.
    // Warnings contiene materias que no se pudieron asignar.
    // Message permite mostrar un resumen general.
    public class GenerateScheduleResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int TotalSubjectsRequested { get; set; }

        public int TotalSubjectsScheduled { get; set; }

        public List<GeneratedScheduleEntryDto> GeneratedSchedules { get; set; } = new();

        public List<string> Warnings { get; set; } = new();
    }
}
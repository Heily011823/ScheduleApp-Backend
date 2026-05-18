// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Application.DTOs
{
    // DTO de entrada para generar automaticamente un horario.
    // El coordinador selecciona programa, semestre y jornada,
    // y el sistema arma el horario para esos parametros.
    public class GenerateScheduleRequestDto
    {
        public Guid AcademicProgramId { get; set; }
        public int SemesterNumber { get; set; }
        public string Shift { get; set; } = string.Empty;
    }
}

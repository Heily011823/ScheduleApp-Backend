// Autor: Jacobo
// Version: 0.1

using System;

namespace ScheduleApp.Application.DTOs
{
    
    public class GenerateScheduleRequestDto
    {
        public Guid AcademicProgramId { get; set; }

        public int SemesterNumber { get; set; }

        public string Shift { get; set; } = string.Empty;
    }
}
// Autor: Jacobo
// Version: 0.1

namespace ScheduleApp.Application.DTOs
{
    // DTO para crear un programa academico.
    public class CreateAcademicProgramDto
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Shift { get; set; } = string.Empty;

        public int TotalSemesters { get; set; }
    }
}
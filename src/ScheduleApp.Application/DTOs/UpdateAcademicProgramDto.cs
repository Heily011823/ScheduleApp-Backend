// Autor: Jacobo
// Version: 0.2

namespace ScheduleApp.Application.DTOs
{
    public class UpdateAcademicProgramDto
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Shift { get; set; } = string.Empty;

        public int TotalSemesters { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
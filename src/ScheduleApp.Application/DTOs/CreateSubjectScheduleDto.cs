namespace ScheduleApp.Application.DTOs
{
    public class CreateSubjectScheduleDto
    {
        public Guid TeacherId { get; set; }

        public Guid ClassroomId { get; set; }

        public string Day { get; set; }
            = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string AcademicProgram { get; set; }
            = string.Empty;

        public string Shift { get; set; }
            = string.Empty;

        public int Semester { get; set; }
    }
}
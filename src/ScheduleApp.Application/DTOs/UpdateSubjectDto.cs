namespace ScheduleApp.Application.DTOs
{
    public class UpdateSubjectDto
    {
        public string Code { get; set; }
            = string.Empty;

        public string Name { get; set; }
            = string.Empty;

        public int Semester { get; set; }

        public int Credits { get; set; }

        public int WeeklyHours { get; set; }

        public bool IsTapsi { get; set; }

        public bool IsActive { get; set; }

        public List<CreateSubjectScheduleDto>
            Schedules
        { get; set; }
                = new();
    }
}
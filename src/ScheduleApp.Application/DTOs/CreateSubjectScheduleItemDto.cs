namespace ScheduleApp.Application.DTOs
{
    public class CreateSubjectScheduleItemDto
    {
        public Guid SubjectId { get; set; }

        public string Day { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
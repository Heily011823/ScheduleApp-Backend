namespace ScheduleApp.Application.DTOs;

public class CreateSubjectRestrictionDto
{
    public Guid SubjectId { get; set; }

    public int Day { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string RestrictionType { get; set; } = string.Empty;
}
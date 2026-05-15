namespace ScheduleApp.Application.DTOs;

public class CreateSubjectDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Semester { get; set; }

    public int Credits { get; set; }

    public int WeeklyHours { get; set; }
}
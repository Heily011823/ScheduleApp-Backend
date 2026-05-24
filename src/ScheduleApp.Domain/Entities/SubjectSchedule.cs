namespace ScheduleApp.Domain.Entities;

public class SubjectSchedule
{
    public Guid Id { get; set; }

    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public string DayOfWeek { get; set; } = string.Empty; 

    public TimeSpan StartHour { get; set; }

    public TimeSpan EndHour { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
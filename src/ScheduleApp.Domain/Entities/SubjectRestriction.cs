namespace ScheduleApp.Domain.Entities;

public class SubjectRestriction
{
    public Guid Id { get; set; }

    public Guid SubjectId { get; set; }

    public int Day { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string RestrictionType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    // RELACIÓN
    public Subject Subject { get; set; } = null!;
}
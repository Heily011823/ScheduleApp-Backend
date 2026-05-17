using System;

namespace ScheduleApp.Domain.Entities;

public class TeacherAvailability
{
    public Guid Id { get; set; }

    
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public DayOfWeek Day { get; set; } 
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxTeachingHours { get; set; }
}
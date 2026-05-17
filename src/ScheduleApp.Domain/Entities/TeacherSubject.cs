using System;

namespace ScheduleApp.Domain.Entities;

public class TeacherSubject
{
    
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public string ContractType { get; set; } = string.Empty; 
}
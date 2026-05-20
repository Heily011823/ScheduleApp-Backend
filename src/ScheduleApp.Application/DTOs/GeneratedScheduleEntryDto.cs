// Autor: Jacobo
// Version: 0.1

using System;

namespace ScheduleApp.Application.DTOs
{
 
    public class GeneratedScheduleEntryDto
    {
        public Guid Id { get; set; }

        public Guid SubjectId { get; set; }

        public string SubjectCode { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public int Credits { get; set; }

        public int WeeklyHours { get; set; }

       
        public bool IsTapsi { get; set; }

        public Guid TeacherId { get; set; }

        public string TeacherFullName { get; set; } = string.Empty;

        public Guid ClassroomId { get; set; }

        public string ClassroomCode { get; set; } = string.Empty;

        public string ClassroomName { get; set; } = string.Empty;

     
        public int Day { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string AcademicProgram { get; set; } = string.Empty;

        public string Shift { get; set; } = string.Empty;

        public int Semester { get; set; }

        public string Status { get; set; } = "Draft";
    }
}
// Autor: Jacobo
// Version: 0.2

using System;

namespace ScheduleApp.Application.DTOs
{
    public class CreateAssignmentDto
    {
        public string Teacher { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Classroom { get; set; } = string.Empty;

        public int Day { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
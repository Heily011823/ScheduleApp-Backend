using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Domain.Entities
{
    public class ClassroomAssignment
    {
        public int Id { get; set; }

        public int ClassroomId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public Status Status { get; set; }

    }
}

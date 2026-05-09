using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Interfaces
{
    public interface IClassroomAvailabilityService
    {
        bool IsAvailable(
            int classroomId,
            DayOfWeek day,
            TimeSpan startTime,
            TimeSpan endTime
        );

        string SaveAssignment(
            ClassroomAssignment newAssignment
        );
    }
}

using ScheduleApp.Domain.Entities;
using System;

namespace ScheduleApp.Application.Interfaces
{
    public interface IClassroomAvailabilityService
    {
       
        bool IsAvailable(
            Guid classroomId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime
        );

        string SaveAssignment(
            ClassroomAssignment newAssignment
        );
    }
}
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
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime
         );

        string SaveAssignment(
            ClassroomAssignment newAssignment
        );
       
    }
}

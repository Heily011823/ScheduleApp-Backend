using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ScheduleApp.Application.Services
{
    public class ClassroomAvailabilityService
    {
        private List<ClassroomAssignment> assignments = new List<ClassroomAssignment>();

        public bool IsAvailable(
            int classroomId,
            DayOfWeek day,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            return !assignments.Any(a =>

                a.ClassroomId == classroomId && a.Day == day && (startTime < a.EndTime && endTime > a.StartTime)
            );
        }

        public string SaveAssignment(
            ClassroomAssignment newAssignment)
        {
            if (!IsAvailable(
                newAssignment.ClassroomId,
                newAssignment.Day,
                newAssignment.StartTime,
                newAssignment.EndTime))
            {
                return
                    "El aula ya está ocupada en ese horario";
            }

            assignments.Add(newAssignment);

            return
                "El aula se asigno correctamente";
        }
    }

}

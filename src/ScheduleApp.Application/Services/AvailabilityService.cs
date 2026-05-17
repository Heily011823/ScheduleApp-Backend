using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class AvailabilityService
    {
        private readonly List<Assignment> assignments = new();

        public string SaveAssignment(Assignment newAssignment)
        {
            bool teacherConflict = assignments.Any(a =>
                a.Teacher == newAssignment.Teacher &&
                a.Day == newAssignment.Day &&
                newAssignment.StartTime < a.EndTime &&
                newAssignment.EndTime > a.StartTime
            );

            if (teacherConflict)
            {
                return "El docente ya tiene una clase en ese horario";
            }

            bool classroomConflict = assignments.Any(a =>
                a.Classroom == newAssignment.Classroom &&
                a.Day == newAssignment.Day &&
                newAssignment.StartTime < a.EndTime &&
                newAssignment.EndTime > a.StartTime
            );

            if (classroomConflict)
            {
                return "El aula ya está ocupada en ese horario";
            }

            assignments.Add(newAssignment);

            return "Asignación guardada correctamente";
        }
    }
}
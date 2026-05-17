using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ScheduleApp.Application.Services
{
    public class ClassroomAvailabilityService : IClassroomAvailabilityService
    {
        private readonly List<ClassroomAssignment> assignments =
            new List<ClassroomAssignment>();

        public bool IsAvailable(
            int classroomId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            if (endTime <= startTime)
            {
                throw new ArgumentException("La hora de fin debe ser mayor a la de inicio.");
            }

            return !assignments.Any(a =>
                a.ClassroomId == classroomId &&
                a.Date.Date == date.Date &&
                (startTime < a.EndTime && endTime > a.StartTime)
            );
        }

        public string SaveAssignment(ClassroomAssignment newAssignment)
        {

            if (newAssignment.Status == Status.Inactiva)
            {
                return "No se puede asignar un aula inactiva.";
            }

            if (newAssignment.EndTime <= newAssignment.StartTime)
            {
                return "La hora de fin debe ser mayor a la de inicio.";
            }

            Console.WriteLine($"NUEVO: {newAssignment.StartTime}-{newAssignment.EndTime}");


            bool available = IsAvailable(
                newAssignment.ClassroomId,
                newAssignment.Date,
                newAssignment.StartTime,
                newAssignment.EndTime
            );

            if (!available)
            {
                return $"El aula ya está ocupada el {newAssignment.Date:yyyy-MM-dd} entre {newAssignment.StartTime} y {newAssignment.EndTime}.";
            }

            assignments.Add(newAssignment);


            return "El aula se asignó correctamente";
        }
    }

}

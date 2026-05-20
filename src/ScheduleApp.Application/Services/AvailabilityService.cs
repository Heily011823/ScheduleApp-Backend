using System;
using System.Collections.Generic;
using System.Linq;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services;

/*
 * Author: Team ScheduleApp
 * Description: Validates and manages classroom schedule overlaps using Guid
 */
public class AvailabilityService : IClassroomAvailabilityService
{
    private readonly List<ClassroomAssignment> assignments = new();

    public bool IsAvailable(Guid classroomId, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        // Verifica si el aula ya está ocupada en ese día y rango horario
        bool hasConflict = assignments.Any(a =>
            a.ClassroomId == classroomId &&
            a.Date.Date == date.Date &&
            startTime < a.EndTime &&
            endTime > a.StartTime
        );

        return !hasConflict;
    }

    public string SaveAssignment(ClassroomAssignment newAssignment)
    {
        // VALIDACIÓN: Usamos el método IsAvailable para comprobar el aula
        if (!IsAvailable(newAssignment.ClassroomId, newAssignment.Date, newAssignment.StartTime, newAssignment.EndTime))
        {
            return "El aula ya está ocupada en ese horario";
        }

        // Si el aula está libre, se guarda
        assignments.Add(newAssignment);

        return "Asignación guardada correctamente";
    }
}
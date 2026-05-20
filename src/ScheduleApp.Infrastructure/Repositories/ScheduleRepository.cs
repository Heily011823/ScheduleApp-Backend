// Autor: Jacobo
// Version: 0.4 - HU-74 fallback docente para materias sin TeacherSubject

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

// Repositorio para gestion de horarios.
// Incluye generacion automatica (HU-74), guardado (HU-71), consulta filtrada
// y los metodos auxiliares para la validacion de creditos (HU-143).
public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    private static readonly TimeSpan ClassDuration = new TimeSpan(1, 0, 0);
    private static readonly TimeSpan DayStart = new TimeSpan(7, 0, 0);
    private static readonly TimeSpan DayEnd = new TimeSpan(12, 0, 0);
    private const int FirstDay = 1;
    private const int LastDay = 5;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    // HU-143 (Mateo Quintero): retorna horarios de un programa y semestre con la materia incluida
    public async Task<IEnumerable<Schedule>> GetByProgramAndSemesterAsync(
        string academicProgram,
        int semester)
    {
        return await _context.Schedules
            .Include(s => s.Subject)
            .Where(s =>
                s.AcademicProgram.ToLower()
                    .Contains(academicProgram.ToLower()) &&
                s.Semester == semester)
            .ToListAsync();
    }

    // HU-143 (Mateo Quintero): crea un horario individual
    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        schedule.Id = Guid.NewGuid();
        schedule.CreatedAt = DateTime.UtcNow;
        await _context.Schedules.AddAsync(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> AcademicProgramExistsAsync(Guid academicProgramId)
    {
        return await _context.AcademicPrograms
            .AnyAsync(program => program.Id == academicProgramId && program.IsActive);
    }

    public async Task<List<GeneratedScheduleEntryDto>> GetSubjectsForGenerationAsync(
        Guid academicProgramId,
        int semesterNumber,
        string shift)
    {
        var academicProgram = await _context.AcademicPrograms
            .FirstOrDefaultAsync(p => p.Id == academicProgramId && p.IsActive);

        if (academicProgram == null)
            return new List<GeneratedScheduleEntryDto>();

        var subjects = await _context.Subjects
            .Where(s => s.Semester == semesterNumber && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        if (!subjects.Any())
            return new List<GeneratedScheduleEntryDto>();

        var classrooms = await _context.Classrooms
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .ToListAsync();

        if (!classrooms.Any())
            return new List<GeneratedScheduleEntryDto>();

        var teacherSubjects = await _context.TeacherSubjects
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .Where(ts =>
                ts.Teacher!.IsActive &&
                ts.Subject!.Semester == semesterNumber &&
                ts.Subject.IsActive)
            .ToListAsync();

        // HU-74 fix: si una materia no tiene docente asignado en TeacherSubjects,
        // usamos un docente activo cualquiera como fallback.
        // Esto evita que se omitan materias cuando la tabla TeacherSubjects
        // todavia no esta poblada (caso comun en datos iniciales del proyecto).
        var fallbackTeachers = await _context.Teachers
            .Where(t => t.IsActive)
            .ToListAsync();

        var existingSchedules = await _context.Schedules.ToListAsync();

        var generatedSchedules = new List<GeneratedScheduleEntryDto>();

        foreach (var subject in subjects)
        {
            var teacherSubject = teacherSubjects
                .FirstOrDefault(ts => ts.SubjectId == subject.Id);

            // Preferir el docente asignado oficialmente; si no hay, usar fallback.
            var teacher = teacherSubject?.Teacher ?? fallbackTeachers.FirstOrDefault();

            if (teacher == null)
                continue;

            bool assigned = false;

            for (int day = FirstDay; day <= LastDay && !assigned; day++)
            {
                for (var slotStart = DayStart;
                     slotStart < DayEnd && !assigned;
                     slotStart = slotStart.Add(ClassDuration))
                {
                    var slotEnd = slotStart.Add(ClassDuration);

                    foreach (var classroom in classrooms)
                    {
                        if (IsTeacherBusy(
                                existingSchedules,
                                generatedSchedules,
                                teacher.Id,
                                day,
                                slotStart,
                                slotEnd))
                        {
                            break;
                        }

                        if (IsClassroomBusy(
                                existingSchedules,
                                generatedSchedules,
                                classroom.Id,
                                day,
                                slotStart,
                                slotEnd))
                        {
                            continue;
                        }

                        generatedSchedules.Add(new GeneratedScheduleEntryDto
                        {
                            Id = Guid.NewGuid(),

                            SubjectId = subject.Id,
                            SubjectCode = subject.Code,
                            SubjectName = subject.Name,
                            Credits = subject.Credits,
                            WeeklyHours = subject.WeeklyHours,
                            IsTapsi = subject.IsTapsi,

                            TeacherId = teacher.Id,
                            TeacherFullName = $"{teacher.FirstName} {teacher.LastName}",

                            ClassroomId = classroom.Id,
                            ClassroomCode = classroom.Code,
                            ClassroomName = classroom.Name,

                            Day = day,
                            StartTime = slotStart,
                            EndTime = slotEnd,

                            AcademicProgram = academicProgram.Name,
                            Shift = shift,
                            Semester = semesterNumber,
                            Status = "Draft"
                        });

                        assigned = true;
                        break;
                    }
                }
            }
        }

        return generatedSchedules;
    }

    public async Task SaveAsync(List<GeneratedScheduleEntryDto> schedules)
    {
        if (schedules == null || schedules.Count == 0)
            return;

        var first = schedules.First();

        var existingSchedules = await _context.Schedules
            .Where(s =>
                s.AcademicProgram == first.AcademicProgram &&
                s.Shift == first.Shift &&
                s.Semester == first.Semester)
            .ToListAsync();

        if (existingSchedules.Any())
        {
            _context.Schedules.RemoveRange(existingSchedules);
        }

        var entities = schedules.Select(schedule => new Schedule
        {
            Id = schedule.Id == Guid.Empty ? Guid.NewGuid() : schedule.Id,

            SubjectId = schedule.SubjectId,
            TeacherId = schedule.TeacherId,
            ClassroomId = schedule.ClassroomId,

            Day = (DayOfWeek)schedule.Day,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,

            AcademicProgram = schedule.AcademicProgram,
            Shift = schedule.Shift,
            Semester = schedule.Semester,
            Status = "Saved",

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        }).ToList();

        await _context.Schedules.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(
        string academicProgram,
        string shift,
        int semester)
    {
        return await _context.Schedules
            .Include(s => s.Subject)
            .Include(s => s.Teacher)
            .Include(s => s.Classroom)
            .Where(s =>
                s.AcademicProgram == academicProgram &&
                s.Shift == shift &&
                s.Semester == semester)
            .OrderBy(s => s.Day)
            .ThenBy(s => s.StartTime)
            .Select(s => new GeneratedScheduleEntryDto
            {
                Id = s.Id,

                SubjectId = s.SubjectId,
                SubjectCode = s.Subject != null ? s.Subject.Code : string.Empty,
                SubjectName = s.Subject != null ? s.Subject.Name : string.Empty,
                Credits = s.Subject != null ? s.Subject.Credits : 0,
                WeeklyHours = s.Subject != null ? s.Subject.WeeklyHours : 0,
                IsTapsi = s.Subject != null && s.Subject.IsTapsi,

                TeacherId = s.TeacherId,
                TeacherFullName = s.Teacher != null ? $"{s.Teacher.FirstName} {s.Teacher.LastName}" : string.Empty,

                ClassroomId = s.ClassroomId,
                ClassroomCode = s.Classroom != null ? s.Classroom.Code : string.Empty,
                ClassroomName = s.Classroom != null ? s.Classroom.Name : string.Empty,

                Day = (int)s.Day,
                StartTime = s.StartTime,
                EndTime = s.EndTime,

                AcademicProgram = s.AcademicProgram,
                Shift = s.Shift,
                Semester = s.Semester,
                Status = s.Status
            })
            .ToListAsync();
    }

    private static bool IsTeacherBusy(
        IEnumerable<Schedule> existingSchedules,
        IEnumerable<GeneratedScheduleEntryDto> generatedSchedules,
        Guid teacherId,
        int day,
        TimeSpan start,
        TimeSpan end)
    {
        bool inExisting = existingSchedules.Any(s =>
            s.TeacherId == teacherId &&
            (int)s.Day == day &&
            s.StartTime < end &&
            s.EndTime > start);

        if (inExisting)
            return true;

        bool inGenerated = generatedSchedules.Any(s =>
            s.TeacherId == teacherId &&
            s.Day == day &&
            s.StartTime < end &&
            s.EndTime > start);

        return inGenerated;
    }

    private static bool IsClassroomBusy(
        IEnumerable<Schedule> existingSchedules,
        IEnumerable<GeneratedScheduleEntryDto> generatedSchedules,
        Guid classroomId,
        int day,
        TimeSpan start,
        TimeSpan end)
    {
        bool inExisting = existingSchedules.Any(s =>
            s.ClassroomId == classroomId &&
            (int)s.Day == day &&
            s.StartTime < end &&
            s.EndTime > start);

        if (inExisting)
            return true;

        bool inGenerated = generatedSchedules.Any(s =>
            s.ClassroomId == classroomId &&
            s.Day == day &&
            s.StartTime < end &&
            s.EndTime > start);

        return inGenerated;
    }
}

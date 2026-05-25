// Autor: Jacobo
// Version: 0.8 - Rotacion de aulas y horario diurno 07-18

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

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    private static readonly TimeSpan SlotStep   = TimeSpan.FromHours(1);
    private static readonly TimeSpan LunchStart = TimeSpan.FromHours(12);
    private static readonly TimeSpan LunchEnd   = TimeSpan.FromHours(14);
    private const int FirstDay = 1;
    private const int LastDay  = 6;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetByProgramAndSemesterAsync(
        string academicProgram, int semester)
    {
        return await _context.Schedules
            .Include(s => s.Subject)
            .Where(s => s.AcademicProgram.ToLower().Contains(academicProgram.ToLower())
                     && s.Semester == semester)
            .ToListAsync();
    }

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
            .AnyAsync(p => p.Id == academicProgramId && p.IsActive);
    }

    public async Task<List<GeneratedScheduleEntryDto>> GetSubjectsForGenerationAsync(
        Guid academicProgramId, int semesterNumber, string shift)
    {
        var academicProgram = await _context.AcademicPrograms
            .FirstOrDefaultAsync(p => p.Id == academicProgramId && p.IsActive);
        if (academicProgram == null) return new();

        var subjects = await _context.Subjects
            .Where(s => s.Semester == semesterNumber && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
        if (!subjects.Any()) return new();

        var classrooms = await _context.Classrooms
            .Where(c => c.IsActive).OrderBy(c => c.Code).ToListAsync();
        if (!classrooms.Any()) return new();

        var teacherSubjects = await _context.TeacherSubjects
            .Include(ts => ts.Teacher).ThenInclude(t => t!.TeacherSpecialties)
            .Include(ts => ts.Subject)
            .Where(ts => ts.Teacher!.IsActive
                      && ts.Subject!.Semester == semesterNumber
                      && ts.Subject.IsActive)
            .ToListAsync();

        var fallbackTeachers = await _context.Teachers
            .Include(t => t.TeacherSpecialties).ThenInclude(ts => ts.Specialty)
            .Where(t => t.IsActive)
            .ToListAsync();

        var existingSchedules = await _context.Schedules.ToListAsync();
        var generatedSchedules = new List<GeneratedScheduleEntryDto>();

        // Horario segun jornada
        bool nocturna = shift?.ToLower().Contains("nocturna") == true;
        var dayStart = nocturna ? TimeSpan.FromHours(17) : TimeSpan.FromHours(7);
        var dayEnd   = nocturna ? TimeSpan.FromHours(21) : TimeSpan.FromHours(18);

        // Rotacion de aulas para no asignar siempre la misma
        int aulaIdx = 0;

        foreach (var subject in subjects)
        {
            // --- Asignacion de docente ---
            var teacherSubject = teacherSubjects.FirstOrDefault(ts => ts.SubjectId == subject.Id);
            Teacher? teacher = teacherSubject?.Teacher;

            if (teacher == null && subject.SpecialtyId.HasValue)
                teacher = fallbackTeachers.FirstOrDefault(t =>
                    t.TeacherSpecialties.Any(ts => ts.SpecialtyId == subject.SpecialtyId));

            if (teacher == null)
                teacher = fallbackTeachers.FirstOrDefault();

            if (teacher == null)
                continue;

            // --- Duracion de cada sesion ---
            int horasXSesion = subject.WeeklyHours == 3 ? 3
                             : subject.WeeklyHours == 1 ? 1
                             : 2;
            var duracionSesion   = TimeSpan.FromHours(horasXSesion);
            int sesionesNecesarias = (int)Math.Ceiling((double)subject.WeeklyHours / horasXSesion);
            int sesionesColocadas  = 0;

            for (int pasada = 0; pasada < 2 && sesionesColocadas < sesionesNecesarias; pasada++)
            {
                for (int day = FirstDay; day <= LastDay && sesionesColocadas < sesionesNecesarias; day++)
                {
                    if (pasada == 0 && generatedSchedules.Any(s => s.SubjectId == subject.Id && s.Day == day))
                        continue;

                    for (var slotStart = dayStart; slotStart < dayEnd; slotStart += SlotStep)
                    {
                        var slotEnd = slotStart + duracionSesion;

                        if (slotEnd > dayEnd)
                            break;

                        // Sin clases de 12:00 a 14:00
                        if (slotStart < LunchEnd && slotEnd > LunchStart)
                            continue;

                        // Sin conflicto con otra materia del grupo
                        if (generatedSchedules.Any(s =>
                                s.Day == day &&
                                s.StartTime < slotEnd &&
                                s.EndTime > slotStart))
                            continue;

                        if (IsTeacherBusy(existingSchedules, generatedSchedules,
                                teacher.Id, day, slotStart, slotEnd))
                            continue;

                        // Buscar aula libre con rotacion para distribuir entre salones
                        Classroom? freeClassroom = null;
                        for (int k = 0; k < classrooms.Count; k++)
                        {
                            var candidate = classrooms[(aulaIdx + k) % classrooms.Count];
                            if (!IsClassroomBusy(existingSchedules, generatedSchedules,
                                    candidate.Id, day, slotStart, slotEnd))
                            {
                                freeClassroom = candidate;
                                break;
                            }
                        }

                        if (freeClassroom == null)
                            continue;

                        generatedSchedules.Add(new GeneratedScheduleEntryDto
                        {
                            Id              = Guid.NewGuid(),
                            SubjectId       = subject.Id,
                            SubjectCode     = subject.Code,
                            SubjectName     = subject.Name,
                            Credits         = subject.Credits,
                            WeeklyHours     = subject.WeeklyHours,
                            IsTapsi         = subject.IsTapsi,
                            TeacherId       = teacher.Id,
                            TeacherFullName = $"{teacher.FirstName} {teacher.LastName}",
                            ClassroomId     = freeClassroom.Id,
                            ClassroomCode   = freeClassroom.Code,
                            ClassroomName   = freeClassroom.Name,
                            Day             = day,
                            StartTime       = slotStart,
                            EndTime         = slotEnd,
                            AcademicProgram = academicProgram.Name,
                            Shift           = shift,
                            Semester        = semesterNumber,
                            Status          = "Draft"
                        });

                        aulaIdx++;
                        sesionesColocadas++;
                        break;
                    }
                }
            }
        }

        return generatedSchedules;
    }

    public async Task SaveAsync(List<GeneratedScheduleEntryDto> schedules)
    {
        if (schedules == null || schedules.Count == 0) return;

        var first = schedules.First();
        var existing = await _context.Schedules
            .Where(s => s.AcademicProgram == first.AcademicProgram
                     && s.Shift == first.Shift
                     && s.Semester == first.Semester)
            .ToListAsync();

        if (existing.Any())
            _context.Schedules.RemoveRange(existing);

        var entities = schedules.Select(s => new Schedule
        {
            Id              = s.Id == Guid.Empty ? Guid.NewGuid() : s.Id,
            SubjectId       = s.SubjectId,
            TeacherId       = s.TeacherId,
            ClassroomId     = s.ClassroomId,
            Day             = (DayOfWeek)s.Day,
            StartTime       = s.StartTime,
            EndTime         = s.EndTime,
            AcademicProgram = s.AcademicProgram,
            Shift           = s.Shift,
            Semester        = s.Semester,
            Status          = "Saved",
            CreatedAt       = DateTime.UtcNow
        }).ToList();

        await _context.Schedules.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(
        string academicProgram, string shift, int semester, string status)
    {
        return await _context.Schedules
            .Include(s => s.Subject)
            .Include(s => s.Teacher)
            .Include(s => s.Classroom)
            .Where(s =>
                (string.IsNullOrEmpty(academicProgram) || s.AcademicProgram == academicProgram) &&
                (string.IsNullOrEmpty(shift)           || s.Shift == shift) &&
                (semester <= 0                         || s.Semester == semester) &&
                (string.IsNullOrEmpty(status)          || s.Status == status))
            .OrderBy(s => s.Day).ThenBy(s => s.StartTime)
            .Select(s => new GeneratedScheduleEntryDto
            {
                Id              = s.Id,
                SubjectId       = s.SubjectId,
                SubjectCode     = s.Subject != null ? s.Subject.Code    : string.Empty,
                SubjectName     = s.Subject != null ? s.Subject.Name    : string.Empty,
                Credits         = s.Subject != null ? s.Subject.Credits : 0,
                WeeklyHours     = s.Subject != null ? s.Subject.WeeklyHours : 0,
                IsTapsi         = s.Subject != null && s.Subject.IsTapsi,
                TeacherId       = s.TeacherId,
                TeacherFullName = s.Teacher != null ? $"{s.Teacher.FirstName} {s.Teacher.LastName}" : string.Empty,
                ClassroomId     = s.ClassroomId,
                ClassroomCode   = s.Classroom != null ? s.Classroom.Code : string.Empty,
                ClassroomName   = s.Classroom != null ? s.Classroom.Name : string.Empty,
                Day             = (int)s.Day,
                StartTime       = s.StartTime,
                EndTime         = s.EndTime,
                AcademicProgram = s.AcademicProgram,
                Shift           = s.Shift,
                Semester        = s.Semester,
                Status          = s.Status
            })
            .ToListAsync();
    }

    private static bool IsTeacherBusy(
        IEnumerable<Schedule> existing, IEnumerable<GeneratedScheduleEntryDto> generated,
        Guid teacherId, int day, TimeSpan start, TimeSpan end)
    {
        return existing.Any(s => s.TeacherId == teacherId && (int)s.Day == day
                              && s.StartTime < end && s.EndTime > start)
            || generated.Any(s => s.TeacherId == teacherId && s.Day == day
                              && s.StartTime < end && s.EndTime > start);
    }

    private static bool IsClassroomBusy(
        IEnumerable<Schedule> existing, IEnumerable<GeneratedScheduleEntryDto> generated,
        Guid classroomId, int day, TimeSpan start, TimeSpan end)
    {
        return existing.Any(s => s.ClassroomId == classroomId && (int)s.Day == day
                              && s.StartTime < end && s.EndTime > start)
            || generated.Any(s => s.ClassroomId == classroomId && s.Day == day
                              && s.StartTime < end && s.EndTime > start);
    }
}

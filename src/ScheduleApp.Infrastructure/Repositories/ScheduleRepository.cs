// Autor: Jacobo
// Version: 0.2 - HU-74 validacion de cruces de docentes y aulas

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories
{
    // Repositorio del modulo de generacion de horarios.
    //
    // Mejoras de HU-74:
    //   - Antes de asignar una materia se valida que el docente no este ocupado
    //     en ese slot (en BD o en lo que ya se genero en esta corrida).
    //   - Tambien se valida que el aula no este ocupada en ese slot.
    //   - Si el slot esta ocupado, el algoritmo prueba la siguiente combinacion
    //     (otro horario u otra aula) hasta encontrar uno libre.
    //   - Si no encuentra slot libre para la materia, se omite (no se agrega al
    //     resultado). El service que llama a este repo puede registrar warnings.
    //
    // Solapamiento de intervalos (formula clasica):
    //   Dos rangos [a.start, a.end] y [b.start, b.end] se solapan si
    //   a.StartTime < b.EndTime AND a.EndTime > b.StartTime.
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

        // Configuracion del algoritmo greedy
        private static readonly TimeSpan ClassDuration = new TimeSpan(1, 0, 0);
        private static readonly TimeSpan DayStart = new TimeSpan(7, 0, 0);
        private static readonly TimeSpan DayEnd = new TimeSpan(12, 0, 0);
        private const int FirstDay = 1; // Lunes
        private const int LastDay = 5;  // Viernes

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
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
                    ts.Teacher.IsActive &&
                    ts.Subject.Semester == semesterNumber &&
                    ts.Subject.IsActive)
                .ToListAsync();

            // Cargar Schedules existentes en BD. Son la base para validar
            // que un docente o aula ya este ocupado en algun slot.
            var existingSchedules = await _context.Schedules.ToListAsync();

            var generatedSchedules = new List<GeneratedScheduleEntryDto>();

            // Para cada materia, buscar la primera combinacion (dia + hora + aula)
            // que este libre tanto para el docente como para el aula.
            foreach (var subject in subjects)
            {
                var teacherSubject = teacherSubjects
                    .FirstOrDefault(ts => ts.SubjectId == subject.Id);

                if (teacherSubject == null)
                    continue;

                var teacher = teacherSubject.Teacher;
                bool assigned = false;

                // Probar slots: dia 1..5 (lunes a viernes), hora 7..11
                for (int day = FirstDay; day <= LastDay && !assigned; day++)
                {
                    for (var slotStart = DayStart;
                         slotStart < DayEnd && !assigned;
                         slotStart = slotStart.Add(ClassDuration))
                    {
                        var slotEnd = slotStart.Add(ClassDuration);

                        // Probar cada aula en este slot
                        foreach (var classroom in classrooms)
                        {
                            if (IsTeacherBusy(
                                    existingSchedules, generatedSchedules,
                                    teacher.Id, day, slotStart, slotEnd))
                            {
                                // El docente ya tiene otra clase en este slot,
                                // saltar a la siguiente aula (no sirve, el docente
                                // es el limitante en este horario).
                                break;
                            }

                            if (IsClassroomBusy(
                                    existingSchedules, generatedSchedules,
                                    classroom.Id, day, slotStart, slotEnd))
                            {
                                // El aula esta ocupada, probar la siguiente
                                continue;
                            }

                            // Slot libre, asignar
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

                // Si !assigned: no se encontro slot libre para esta materia.
                // No se agrega al resultado. El service deberia registrar un warning
                // pero por contrato del IScheduleRepository solo retornamos los que
                // si se pudieron asignar.
            }

            return generatedSchedules;
        }

        // ============================================================
        // VALIDACIONES DE CRUCES (HU-74)
        // ============================================================

        // El docente esta ocupado en (day, start, end) si tiene un Schedule
        // existente en BD o un Schedule recien generado que solape ese rango.
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
                s.StartTime < end && s.EndTime > start);

            if (inExisting) return true;

            bool inGenerated = generatedSchedules.Any(s =>
                s.TeacherId == teacherId &&
                s.Day == day &&
                s.StartTime < end && s.EndTime > start);

            return inGenerated;
        }

        // El aula esta ocupada en (day, start, end) si tiene un Schedule
        // existente en BD o un Schedule recien generado que solape ese rango.
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
                s.StartTime < end && s.EndTime > start);

            if (inExisting) return true;

            bool inGenerated = generatedSchedules.Any(s =>
                s.ClassroomId == classroomId &&
                s.Day == day &&
                s.StartTime < end && s.EndTime > start);

            return inGenerated;
        }
    }
}

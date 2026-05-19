// Autor: Jacobo
// Version: 0.1

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

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
                .FirstOrDefaultAsync(program =>
                    program.Id == academicProgramId &&
                    program.IsActive);

            if (academicProgram == null)
                return new List<GeneratedScheduleEntryDto>();

            var subjects = await _context.Subjects
                .Where(subject =>
                    subject.Semester == semesterNumber &&
                    subject.IsActive)
                .OrderBy(subject => subject.Name)
                .ToListAsync();

            if (!subjects.Any())
                return new List<GeneratedScheduleEntryDto>();

            var classrooms = await _context.Classrooms
                .Where(classroom => classroom.IsActive)
                .OrderBy(classroom => classroom.Code)
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

            var generatedSchedules = new List<GeneratedScheduleEntryDto>();

            int day = 1; // 1 = Lunes
            TimeSpan startTime = new TimeSpan(7, 0, 0);
            TimeSpan classDuration = new TimeSpan(1, 0, 0);

            int classroomIndex = 0;

            foreach (var subject in subjects)
            {
                var teacherSubject = teacherSubjects
                    .FirstOrDefault(ts => ts.SubjectId == subject.Id);

                if (teacherSubject == null)
                    continue;

                var classroom = classrooms[classroomIndex];

                generatedSchedules.Add(new GeneratedScheduleEntryDto
                {
                    Id = Guid.NewGuid(),

                    SubjectId = subject.Id,
                    SubjectCode = subject.Code,
                    SubjectName = subject.Name,
                    Credits = subject.Credits,
                    WeeklyHours = subject.WeeklyHours,
                    IsTapsi = subject.IsTapsi,

                    TeacherId = teacherSubject.Teacher.Id,
                    TeacherFullName = $"{teacherSubject.Teacher.FirstName} {teacherSubject.Teacher.LastName}",

                    ClassroomId = classroom.Id,
                    ClassroomCode = classroom.Code,
                    ClassroomName = classroom.Name,

                    Day = day,
                    StartTime = startTime,
                    EndTime = startTime.Add(classDuration),

                    AcademicProgram = academicProgram.Name,
                    Shift = shift,
                    Semester = semesterNumber,
                    Status = "Draft"
                });

                startTime = startTime.Add(classDuration);

                if (startTime >= new TimeSpan(12, 0, 0))
                {
                    startTime = new TimeSpan(7, 0, 0);
                    day++;
                }

                if (day > 6)
                    day = 1;

                classroomIndex++;

                if (classroomIndex >= classrooms.Count)
                    classroomIndex = 0;
            }

            return generatedSchedules;
        }
    }
}
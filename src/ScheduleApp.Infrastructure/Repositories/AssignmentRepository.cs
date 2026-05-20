using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;

        public AssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasTeacherScheduleConflict(
            string teacher,
            int day,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            return await _context.Assignments.AnyAsync(a =>
                a.Teacher == teacher &&
                a.Day == day &&
                startTime < a.EndTime &&
                endTime > a.StartTime
            );
        }

        public async Task<bool> HasTeacherScheduleConflictExcluding(
            string teacher,
            int day,
            TimeSpan startTime,
            TimeSpan endTime,
            int excludeAssignmentId)
        {
            return await _context.Assignments.AnyAsync(a =>
                a.Id != excludeAssignmentId &&
                a.Teacher == teacher &&
                a.Day == day &&
                startTime < a.EndTime &&
                endTime > a.StartTime
            );
        }

        public async Task<bool> HasClassroomScheduleConflict(
            string classroom,
            int day,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            return await _context.Assignments.AnyAsync(a =>
                a.Classroom == classroom &&
                a.Day == day &&
                startTime < a.EndTime &&
                endTime > a.StartTime
            );
        }

        public async Task<bool> HasClassroomScheduleConflictExcluding(
            string classroom,
            int day,
            TimeSpan startTime,
            TimeSpan endTime,
            int excludeAssignmentId)
        {
            return await _context.Assignments.AnyAsync(a =>
                a.Id != excludeAssignmentId &&
                a.Classroom == classroom &&
                a.Day == day &&
                startTime < a.EndTime &&
                endTime > a.StartTime
            );
        }

        public async Task CreateAsync(Assignment assignment)
        {
            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .OrderBy(a => a.Day)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
                return false;

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
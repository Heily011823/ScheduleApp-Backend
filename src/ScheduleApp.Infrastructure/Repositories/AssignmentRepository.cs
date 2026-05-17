using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;


namespace ScheduleApp.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;
        /*
         * Author: Salome Carmona
         * Feature: Teacher Schedule Validation
         * Description: Validates overlapping teacher schedules
         */

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

        /*
        * Author: Salome Carmona
        * Feature: Classroom Availability Validation
        * Description: Checks if classroom already has an assignment
        */

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
        public async Task CreateAsync(Assignment assignment)
        {
            await _context.Assignments.AddAsync(assignment);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Assignment>> GetAllAsync()
        {
            return await _context.Assignments.ToListAsync();
        }
    }
}
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

        public async Task CreateAsync(Assignment assignment)
        {
            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Assignment>> GetAllAsync()
        {
            return await _context.Assignments.ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments.FindAsync(id);
        }

        public async Task UpdateAsync(Assignment assignment)
        {
            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Assignments.FindAsync(id);
            if (existing is null)
                return false;

            _context.Assignments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

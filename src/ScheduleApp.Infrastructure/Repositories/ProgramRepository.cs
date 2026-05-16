using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly AppDbContext _context;

        public ProgramRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Program>> GetAllAsync()
        {
            return await _context.Programs.AsNoTracking().ToListAsync();
        }

        public async Task<Program?> GetByIdAsync(Guid id)
        {
            return await _context.Programs.FindAsync(id);
        }

        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            var trimmed = code.Trim();
            return await _context.Programs
                .Where(p => p.Code == trimmed)
                .Where(p => excludeId == null || p.Id != excludeId)
                .AnyAsync();
        }

        public async Task CreateAsync(Program program)
        {
            await _context.Programs.AddAsync(program);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Program program)
        {
            _context.Programs.Update(program);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.Programs.FindAsync(id);
            if (existing is null)
                return false;

            _context.Programs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

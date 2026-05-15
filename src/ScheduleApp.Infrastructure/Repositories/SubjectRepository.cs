using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;

    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Subject?> GetByIdAsync(Guid id)
    {
        return await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Subject>> GetActiveAsync()
    {
        return await _context.Subjects
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    public async Task UpdateAsync(Subject subject)
    {
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(Subject subject)
    {
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();
    }

    public async Task<Subject?> GetByCodeAsync(string code)
    {
        return await _context.Subjects
            .FirstOrDefaultAsync(s => s.Code == code);
    }
}
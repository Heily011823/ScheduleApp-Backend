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
        // Solo retorna las materias no borradas
        return await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    // Obtiene cualquier registro, incluso uno borrado
    public async Task<Subject?> GetByIdAsyncWithoutFilter(Guid id)
    {
        return await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Subject>> GetActiveAsync()
    {
        return await _context.Subjects
            .Where(s => s.IsActive && !s.IsDeleted)
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
            .FirstOrDefaultAsync(s => s.Code == code && !s.IsDeleted);
    }

    public async Task DeleteLogicalAsync(Guid id)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null)
            throw new KeyNotFoundException("Materia no encontrada.");

        subject.IsDeleted = true;
        subject.IsActive = false;
        subject.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<(List<Subject> Items, int TotalCount)> SearchAsync(
        string? search,
        int? semester,
        bool? isActive,
        int page,
        int pageSize)
    {
        var query = _context.Subjects.AsQueryable();

        // Nunca mostrar materias borradas
        query = query.Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.Name.Contains(search) ||
                s.Code.Contains(search));
        }

        if (semester.HasValue)
        {
            query = query.Where(s =>
                s.Semester == semester.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(s =>
                s.IsActive == isActive.Value);
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

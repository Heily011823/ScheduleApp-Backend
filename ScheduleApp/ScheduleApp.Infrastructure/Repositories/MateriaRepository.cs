using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

public class MateriaRepository : IMateriaRepository
{
    private readonly AppDbContext _context;

    public MateriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Materia?> GetByIdAsync(int id)
    {
        return await _context.Materias
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Materia>> GetActivasAsync()
    {
        return await _context.Materias
            .Where(m => m.Activo)
            .ToListAsync();
    }

    public async Task UpdateAsync(Materia materia)
    {
        _context.Materias.Update(materia);
        await _context.SaveChangesAsync();
    }
}
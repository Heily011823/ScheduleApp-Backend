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

    // MODIFICADO: Cambiamos la firma para aceptar 'page' y 'pageSize', 
    // y retornamos una Tupla (List, int) que requiere el DTO de Jacobo.
    public async Task<(List<Subject> Items, int TotalCount)> SearchAsync(
        string? search,
        int? semester,
        bool? isActive,
        int page,
        int pageSize)
    {
        // 1. Construimos la consulta base como IQueryable (No ejecuta SQL aún)
        var query = _context.Subjects.AsQueryable();

        // 2. Evaluamos y aplicamos los mismos filtros dinámicos que tenías
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

        // 3. Contamos el total de registros en la base de datos que cumplen con los criterios 
        // ¡Crucial hacer esto ANTES de aplicar el Skip y el Take!
        int totalCount = await query.CountAsync();

        // 4. Aplicamos la segmentación matemática para la paginación a nivel de SQL Server
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 5. Devolvemos ambos valores empaquetados en una tupla limpia
        return (items, totalCount);
    }
}
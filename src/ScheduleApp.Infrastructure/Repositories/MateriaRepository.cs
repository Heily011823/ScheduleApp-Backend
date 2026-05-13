using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
namespace ScheduleApp.Infrastructure.Repositories;
/// <summary>
/// Implementacion concreta de IMateriaRepository usando Entity Framework Core.
/// Encapsula las consultas a la tabla Materias.
/// </summary>
public class MateriaRepository : IMateriaRepository
{
    private readonly AppDbContext _context;

    public MateriaRepository(AppDbContext context) => _context = context;

    /// <summary>
    /// Construye dinamicamente la query con los filtros que vengan no nulos.
    /// Si no se pasa ningun filtro, retorna todas las materias.
    /// </summary>
    public async Task<IEnumerable<Materia>> SearchMateriasAsync(
        string? nombre,
        int? semestre,
        bool? isActive)
    {
        var query = _context.Materias.AsQueryable();

        if (!string.IsNullOrEmpty(nombre))
        {
            query = query.Where(m => m.Nombre.Contains(nombre));
        }
        if (semestre.HasValue)
        {
            query = query.Where(m => m.Semestre == semestre.Value);
        }
        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }

        return await query.ToListAsync();
    }
}
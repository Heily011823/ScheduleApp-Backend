using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

// Ruta recomendada: src/ScheduleApp.Infrastructure/Repositories/TeacherRepository.cs
namespace ScheduleApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio de docentes usando Entity Framework Core.
/// Implementa búsquedas y persistencia.
/// </summary>
public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _context;

    public TeacherRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca docentes con filtros opcionales. (Alineado con la interfaz)
    /// </summary>
    public async Task<IEnumerable<Teacher>> SearchAsync(string? name, bool? isActive)
    {
        var query = _context.Teachers.AsQueryable();

        // Filtro por nombre completo (FirstName + LastName)
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t =>
                (t.FirstName + " " + t.LastName)
                    .ToLower()
                    .Contains(name.ToLower()));
        }

        // Filtro por estado (Activo/Inactivo)
        if (isActive.HasValue)
        {
            query = query.Where(t => t.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .ToListAsync();
    }

    /// <summary>
    /// Busca un docente por ID.
    /// </summary>
    public async Task<Teacher?> GetByIdAsync(Guid id)
    {
        return await _context.Teachers.FindAsync(id);
    }

    /// <summary>
    /// Busca un docente por correo electrónico.
    /// </summary>
    public async Task<Teacher?> GetByEmailAsync(string email)
    {
        return await _context.Teachers
            .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower().Trim());
    }

    /// <summary>
    /// Busca un docente por documento de identidad.
    /// </summary>
    public async Task<Teacher?> GetByIdentityDocumentAsync(string identityDocument)
    {
        return await _context.Teachers
            .FirstOrDefaultAsync(t => t.IdentityDocument == identityDocument.Trim());
    }

    /// <summary>
    /// Guarda un nuevo docente.
    /// </summary>
    public async Task AddAsync(Teacher teacher)
    {
        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza un docente existente.
    /// </summary>
    public async Task UpdateAsync(Teacher teacher)
    {
        _context.Teachers.Update(teacher);
        await _context.SaveChangesAsync();
    }
}
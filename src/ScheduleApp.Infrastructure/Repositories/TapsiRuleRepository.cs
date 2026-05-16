using System;
using System.Collections.Generic;
using System.Text;

// ScheduleApp.Infrastructure/Repositories/TapsiRuleRepository.cs
namespace ScheduleApp.Infrastructure.Repositories;


/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;



/// <summary>
/// Repositorio para gestión de reglas TAPSI en base de datos SQL Server.
/// </summary>
public class TapsiRuleRepository : ITapsiRuleRepository
{
    private readonly AppDbContext _context;

    public TapsiRuleRepository(AppDbContext context) => _context = context;

    /// <summary>Retorna todas las reglas TAPSI ordenadas por tipo.</summary>
    public async Task<IEnumerable<TapsiRule>> GetAllAsync() =>
        await _context.TapsiRules
            .OrderBy(r => r.RuleType)
            .ToListAsync();

    /// <summary>Retorna solo las reglas activas.</summary>
    public async Task<IEnumerable<TapsiRule>> GetActiveAsync() =>
        await _context.TapsiRules
            .Where(r => r.IsActive)
            .OrderBy(r => r.RuleType)
            .ToListAsync();

    /// <summary>Busca una regla por su ID.</summary>
    public async Task<TapsiRule?> GetByIdAsync(Guid id) =>
        await _context.TapsiRules.FindAsync(id);

    /// <summary>Guarda una nueva regla en BD.</summary>
    public async Task AddAsync(TapsiRule rule)
    {
        await _context.TapsiRules.AddAsync(rule);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza una regla existente en BD.</summary>
    public async Task UpdateAsync(TapsiRule rule)
    {
        _context.TapsiRules.Update(rule);
        await _context.SaveChangesAsync();
    }

    /// <summary>Elimina una regla de BD.</summary>
    public async Task DeleteAsync(Guid id)
    {
        var rule = await _context.TapsiRules.FindAsync(id);
        if (rule is not null)
        {
            _context.TapsiRules.Remove(rule);
            await _context.SaveChangesAsync();
        }
    }
}

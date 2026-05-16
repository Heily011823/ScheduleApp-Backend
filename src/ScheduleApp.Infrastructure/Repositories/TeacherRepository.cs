using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Infrastructure/Repositories/TeacherRepository.cs
namespace ScheduleApp.Infrastructure.Repositories;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes
/// 

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

/// <summary>
/// Repositorio de docentes usando Entity Framework Core.
/// Implementa búsqueda con filtros dinámicos.
/// </summary>
public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _context;

    public TeacherRepository(AppDbContext context) => _context = context;

    /// <summary>
    /// Busca docentes con filtros opcionales.
    /// Todos los filtros son acumulativos (AND).
    /// </summary>
    public async Task<IEnumerable<Teacher>> SearchAsync(
        string? name,
        string? specialty,
        bool? isActive)
    {
        var query = _context.Teachers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(t =>
                t.FullName.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrWhiteSpace(specialty))
            query = query.Where(t =>
                t.Specialty.ToLower().Contains(specialty.ToLower()));

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        return await query
            .OrderBy(t => t.FullName)
            .ToListAsync();
    }

    /// <summary>Busca un docente por ID.</summary>
    public async Task<Teacher?> GetByIdAsync(Guid id) =>
        await _context.Teachers.FindAsync(id);

    /// <summary>Busca un docente por email (case-insensitive).</summary>
    public async Task<Teacher?> GetByEmailAsync(string email) =>
        await _context.Teachers
            .FirstOrDefaultAsync(t =>
                t.Email == email.ToLower().Trim());

    /// <summary>Busca un docente por documento de identidad.</summary>
    public async Task<Teacher?> GetByIdentityDocumentAsync(string identityDocument) =>
        await _context.Teachers
            .FirstOrDefaultAsync(t =>
                t.IdentityDocument == identityDocument.Trim());

    /// <summary>Guarda un nuevo docente en BD.</summary>
    public async Task AddAsync(Teacher teacher)
    {
        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza un docente existente en BD.</summary>
    public async Task UpdateAsync(Teacher teacher)
    {
        _context.Teachers.Update(teacher);
        await _context.SaveChangesAsync();
    }
}
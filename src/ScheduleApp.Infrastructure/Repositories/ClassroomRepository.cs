// src/ScheduleApp.Infrastructure/Repositories/ClassroomRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

/*
 * Authors: Salome Carmona, Mateo Quintero
 * Description: Entity Framework implementation for classroom data access using Guid
 * Features: Case-insensitive unique code search (#84), Status toggling (#85)
 */
public class ClassroomRepository : IClassroomRepository
{
    private readonly AppDbContext _context;

    public ClassroomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Classroom>> GetAllAsync()
    {
        return await _context.Classrooms.ToListAsync();
    }

    public async Task<Classroom?> GetByIdAsync(Guid id)
    {
        return await _context.Classrooms.FindAsync(id);
    }

    /// <summary>
    /// Busca un aula por su código único de forma segura (ignora mayúsculas y espacios).
    /// </summary>
    public async Task<Classroom?> GetByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        return await _context.Classrooms
            .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower().Trim());
    }

    public async Task CreateAsync(Classroom classroom)
    {
        // Si por alguna razón el Guid no viene asignado desde arriba, lo generamos aquí
        if (classroom.Id == Guid.Empty)
        {
            classroom.Id = Guid.NewGuid();
        }

        await _context.Classrooms.AddAsync(classroom);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Classroom classroom)
    {
        _context.Classrooms.Update(classroom);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var classroom = await _context.Classrooms.FindAsync(id);
        if (classroom != null)
        {
            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
    {
        var classroom = await _context.Classrooms.FindAsync(id);
        if (classroom is null) return null;

        classroom.IsActive = isActive;
        classroom.UpdatedAt = DateTime.UtcNow;

        _context.Classrooms.Update(classroom);
        await _context.SaveChangesAsync();
        return classroom;
    }
}
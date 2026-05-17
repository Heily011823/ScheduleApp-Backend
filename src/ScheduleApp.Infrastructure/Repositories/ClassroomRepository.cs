// src/ScheduleApp.Infrastructure/Repositories/ClassroomRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;


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

    // CORREGIDO: Cambiado de int a Guid
    public async Task<Classroom?> GetByIdAsync(Guid id)
    {
        return await _context.Classrooms.FindAsync(id);
    }

    public async Task<Classroom?> GetByCodeAsync(string code)
    {
        return await _context.Classrooms.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task CreateAsync(Classroom classroom)
    {
        // Si el Guid viene vacío, lo generamos antes de guardar
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
        var classroom = await GetByIdAsync(id);
        if (classroom != null)
        {
            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
        }
    }

  
    public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
    {
        var classroom = await GetByIdAsync(id);
        if (classroom == null) return null;

        classroom.IsActive = isActive;
        _context.Classrooms.Update(classroom);
        await _context.SaveChangesAsync();

        return classroom;
    }
}
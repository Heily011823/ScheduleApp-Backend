using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Infrastructure/Repositories/ScheduleRepository.cs
namespace ScheduleApp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

/// <summary>
/// Repositorio para gestión de horarios.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
/// Rama: 143-validar-creditos-scheduleApp
public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context) => _context = context;

    /// <summary>
    /// Retorna todos los horarios de un programa y semestre.
    /// Incluye la materia para acceder a sus créditos.
    /// </summary>
    public async Task<IEnumerable<Schedule>> GetByProgramAndSemesterAsync(
        string academicProgram,
        int semester)
    {
        return await _context.Schedules
            .Include(s => s.Subject)
            .Where(s =>
                s.AcademicProgram.ToLower()
                    .Contains(academicProgram.ToLower()) &&
                s.Semester == semester)
            .ToListAsync();
    }

    /// <summary>Guarda un nuevo horario en BD.</summary>
    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        schedule.Id = Guid.NewGuid();
        schedule.CreatedAt = DateTime.UtcNow;
        await _context.Schedules.AddAsync(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;



/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: develop
/// <summary>
/// Repositorio para operaciones de base de datos de especialidades.
/// </summary>
public class SpecialtyRepository : ISpecialtyRepository
{
    private readonly AppDbContext _context;

    public SpecialtyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync()
    {
        return await _context.Specialties
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Specialty?> GetSpecialtyByIdAsync(Guid id)
    {
        return await _context.Specialties
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Specialty?> GetSpecialtyByNameAsync(string name)
    {
        return await _context.Specialties
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> SpecialtyExistsAsync(string name)
    {
        return await _context.Specialties
            .AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task AddSpecialtyAsync(Specialty specialty)
    {
        await _context.Specialties.AddAsync(specialty);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSpecialtyAsync(Specialty specialty)
    {
        _context.Specialties.Update(specialty);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasTeachersWithSpecialtyAsync(Guid specialtyId)
    {
        return await _context.TeacherSpecialties
            .AnyAsync(ts => ts.SpecialtyId == specialtyId);
    }
}
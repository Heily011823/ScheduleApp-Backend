
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

/*
 * Author: Salome Carmona
 * Feature: Program Semesters CRUD
 * Description: Repository for querying and updating program semesters
 */

/// <summary>
/// Repositorio para consultar configuración de semestres por programa.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
/// Rama: 143-validar-creditos-scheduleApp
public class ProgramSemesterRepository : IProgramSemesterRepository
{
    private readonly AppDbContext _context;

    // Constructor único
    public ProgramSemesterRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca el límite de créditos configurado para un semestre
    /// específico de un programa académico.
    /// </summary>
    public async Task<ProgramSemester?> GetByProgramAndSemesterAsync(
        string academicProgram,
        int semesterNumber)
    {
        return await _context.ProgramSemesters
            .Include(ps => ps.AcademicProgram)
            .FirstOrDefaultAsync(ps =>
                ps.AcademicProgram!.Name.ToLower()
                    .Contains(academicProgram.ToLower()) &&
                ps.SemesterNumber == semesterNumber);
    }

    public async Task<List<ProgramSemester>> GetByProgramIdAsync(Guid academicProgramId)
    {
        return await _context.ProgramSemesters
            .Where(ps => ps.AcademicProgramId == academicProgramId)
            .OrderBy(ps => ps.SemesterNumber)
            .ToListAsync();
    }

    public async Task UpdateAsync(List<ProgramSemester> semesters)
    {
        _context.ProgramSemesters.UpdateRange(semesters);
        await _context.SaveChangesAsync();
    }
}
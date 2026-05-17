using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Infrastructure/Repositories/ProgramSemesterRepository.cs
namespace ScheduleApp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

/// <summary>
/// Repositorio para consultar configuración de semestres por programa.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
/// Rama: 143-validar-creditos-scheduleApp
public class ProgramSemesterRepository : IProgramSemesterRepository
{
    private readonly AppDbContext _context;

    public ProgramSemesterRepository(AppDbContext context) => _context = context;

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
}

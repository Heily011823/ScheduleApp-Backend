using System;
using System.Collections.Generic;
using System.Text;

using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

/// <summary>
/// Contrato para acceso a datos de semestres por programa académico.
/// </summary>
public interface IProgramSemesterRepository
{
    /// <summary>
    /// Obtiene todos los semestres de un programa específico por su ID.
    /// </summary>
    Task<List<ProgramSemester>> GetByProgramIdAsync(Guid programId);

    /// <summary>
    /// Actualiza la lista de semestres para un programa.
    /// </summary>
    Task UpdateAsync(List<ProgramSemester> semesters);

    /// <summary>
    /// Busca la configuración de un semestre específico de un programa.
    /// Usado para validar el límite de créditos por semestre.
    /// </summary>
    Task<ProgramSemester?> GetByProgramAndSemesterAsync(
        string academicProgram,
        int semesterNumber);
}

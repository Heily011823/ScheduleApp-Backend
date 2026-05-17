using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Interfaces/IProgramSemesterRepository.cs
namespace ScheduleApp.Application.Interfaces;

/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: 143-validar-creditos


using ScheduleApp.Domain.Entities;

/// <summary>
/// Contrato para acceso a datos de semestres por programa académico.
/// Usado para validar el límite de créditos por semestre.
/// </summary>
public interface IProgramSemesterRepository
{
    /// <summary>
    /// Busca la configuración de un semestre específico de un programa.
    /// </summary>
    Task<ProgramSemester?> GetByProgramAndSemesterAsync(
        string academicProgram,
        int semesterNumber);
}
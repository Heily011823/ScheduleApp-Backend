
﻿using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Interfaces/IScheduleRepository.cs
namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Application.DTOs;

/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: 143-validar-creditos


// Autor: Jacobo
// Version: 0.2

using ScheduleApp.Domain.Entities;

/// <summary>
/// Contrato para acceso a datos de horarios.
/// </summary>
public interface IScheduleRepository
{
    /// <summary>
    /// Retorna todos los horarios de un programa y semestre específico.
    /// Usado para calcular créditos acumulados.
    /// </summary>
    Task<IEnumerable<Schedule>> GetByProgramAndSemesterAsync(
        string academicProgram,
        int semester);

    /// <summary>Guarda un nuevo horario.</summary>
    Task<Schedule> CreateAsync(Schedule schedule);

    Task<bool> AcademicProgramExistsAsync(Guid academicProgramId);

        Task<List<GeneratedScheduleEntryDto>> GetSubjectsForGenerationAsync(
            Guid academicProgramId,
            int semesterNumber,
            string shift
        );

        Task SaveAsync(
            List<GeneratedScheduleEntryDto> schedules
        );

        Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(
            string academicProgram,
            string shift,
            int semester
        );

}

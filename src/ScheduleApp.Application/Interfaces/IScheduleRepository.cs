using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Interfaces/IScheduleRepository.cs
namespace ScheduleApp.Application.Interfaces;

/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: 143-validar-creditos


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
}

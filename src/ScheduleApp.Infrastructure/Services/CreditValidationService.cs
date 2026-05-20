using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Services/CreditValidationService.cs
namespace ScheduleApp.Application.Services;

using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

/// <summary>
/// Servicio que valida que las materias asignadas no superen
/// el límite de créditos del semestre según dbo.ProgramSemesters.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
/// Rama: 143-validar-creditos-scheduleApp
public class CreditValidationService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IProgramSemesterRepository _programSemesterRepository;
    private readonly ISubjectRepository _subjectRepository;

    public CreditValidationService(
        IScheduleRepository scheduleRepository,
        IProgramSemesterRepository programSemesterRepository,
        ISubjectRepository subjectRepository)
    {
        _scheduleRepository = scheduleRepository;
        _programSemesterRepository = programSemesterRepository;
        _subjectRepository = subjectRepository;
    }

    /// <summary>
    /// Valida que agregar una materia no supere el límite de créditos
    /// del semestre configurado en ProgramSemesters.
    /// Criterio: si total acumulado + nuevos créditos > MaxCredits → 400 Bad Request.
    /// Criterio: si hay espacio disponible → permite guardar.
    /// </summary>
    /// <param name="subjectId">ID de la materia a asignar.</param>
    /// <param name="academicProgram">Nombre del programa académico.</param>
    /// <param name="semester">Número del semestre.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando se supera el límite de créditos del semestre.
    /// </exception>
    public async Task ValidateAsync(
        Guid subjectId,
        string academicProgram,
        int semester)
    {
        // 1. Obtener la materia y sus créditos
        var subject = await _subjectRepository.GetByIdAsync(subjectId)
            ?? throw new KeyNotFoundException(
                $"No se encontró la materia con ID '{subjectId}'.");

        // 2. Obtener el límite de créditos del semestre en ese programa
        var programSemester = await _programSemesterRepository
            .GetByProgramAndSemesterAsync(academicProgram, semester);

        // Si no hay configuración de créditos para ese semestre, no validamos
        if (programSemester is null) return;

        // 3. Obtener los horarios ya asignados en ese programa/semestre
        var existingSchedules = await _scheduleRepository
            .GetByProgramAndSemesterAsync(academicProgram, semester);

        // 4. Calcular créditos acumulados
        var accumulatedCredits = existingSchedules
            .Where(s => s.Subject is not null)
            .Sum(s => s.Subject!.Credits);

        // 5. Validar si agregar la materia supera el límite
        var totalAfterAdding = accumulatedCredits + subject.Credits;

        if (totalAfterAdding > programSemester.MaxCredits)
            throw new InvalidOperationException(
                "Límite de créditos excedido para este semestre.");
    }
}

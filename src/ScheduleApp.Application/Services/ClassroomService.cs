// src/ScheduleApp.Application/Services/ClassroomService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services;

/*
 * Authors: Salome Carmona, Mateo Quintero
 * Description: Implementation of business rules for classrooms using Guid identifiers
 * Features: #84 Unique code validation, #85 Status change management
 */
public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;

    public ClassroomService(IClassroomRepository classroomRepository)
    {
        _classroomRepository = classroomRepository;
    }

    public async Task<List<Classroom>> GetClassroomsAsync()
    {
        return await _classroomRepository.GetAllAsync();
    }

    public async Task<Classroom?> GetClassroomByIdAsync(Guid id)
    {
        return await _classroomRepository.GetByIdAsync(id);
    }

    public async Task CreateClassroomAsync(Classroom classroom)
    {
        // Regla de negocio Criterio #84: Validar código único institucional
        var existing = await _classroomRepository.GetByCodeAsync(classroom.Code);
        if (existing != null)
        {
            throw new InvalidOperationException($"El código de aula '{classroom.Code}' ya está registrado.");
        }

        await _classroomRepository.CreateAsync(classroom);
    }

    public async Task UpdateClassroomAsync(Classroom classroom)
    {
        // Regla de negocio Criterio #84: Validar que el código no lo tenga otra aula diferente
        var existing = await _classroomRepository.GetByCodeAsync(classroom.Code);
        if (existing != null && existing.Id != classroom.Id)
        {
            throw new InvalidOperationException($"El código '{classroom.Code}' ya está siendo usado por otra aula.");
        }

        await _classroomRepository.UpdateAsync(classroom);
    }

    public async Task DeleteClassroomAsync(Guid id)
    {
        await _classroomRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Cambia el estado activo/inactivo de un aula.
    /// Feature #85: Si se desactiva, no aparecerá en las asignaciones disponibles.
    /// </summary>
    public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
    {
        // Buscamos el aula primero para validar su existencia y mutar sus propiedades
        var classroom = await _classroomRepository.GetByIdAsync(id);
        if (classroom is null)
            return null;

        classroom.IsActive = isActive;
        classroom.UpdatedAt = DateTime.UtcNow;

        await _classroomRepository.UpdateAsync(classroom);
        return classroom;
    }
}
// src/ScheduleApp.Application/Services/ClassroomService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services;


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
        // Regla de negocio Criterio #84: Validar código único
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

  
    public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
    {
        return await _classroomRepository.ChangeStatusAsync(id, isActive);
    }
}
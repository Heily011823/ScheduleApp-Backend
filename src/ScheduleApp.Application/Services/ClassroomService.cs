// src/ScheduleApp.Application/Services/ClassroomService.cs
using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Threading.Tasks;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
=======
using System.Text;
using System.Threading.Tasks;
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c

namespace ScheduleApp.Application.Services;


public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;

    public ClassroomService(IClassroomRepository classroomRepository)
    {
<<<<<<< HEAD
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
=======
        private readonly IClassroomRepository _repository;

        public ClassroomService(IClassroomRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Classroom>> GetClassroomsAsync()
        {
            return await _repository.GetAllAsync();
        }

        // CORREGIDO: Cambiado de int a Guid
        public async Task<Classroom?> GetClassroomByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateClassroomAsync(Classroom classroom)
        {
            await _repository.CreateAsync(classroom);
        }

        public async Task UpdateClassroomAsync(Classroom classroom)
        {
            await _repository.UpdateAsync(classroom);
        }

        // CORREGIDO: Cambiado de int a Guid
        public async Task DeleteClassroomAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Cambia el estado activo/inactivo de un aula.
        /// Criterio: si está activa y se desactiva → no aparece en asignaciones.
        /// Criterio: si está inactiva y se activa → vuelve a estar disponible.
        /// </summary>
        /// Autor: Mateo Quintero
        /// Version: 0.1
        /// Rama: 85-implementar-cambio-de-estado-de-aula
        // CORREGIDO: Cambiado de int a Guid
        public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
        {
            var classroom = await _repository.GetByIdAsync(id);
            if (classroom is null) return null;

            classroom.IsActive = isActive;
            classroom.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(classroom);
            return classroom;
        }
    }
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
}
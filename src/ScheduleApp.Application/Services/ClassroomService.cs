using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApp.Application.Services
{
    /*
      * Author: Salome Carmona
      * Feature: Classroom CRUD
      * Description: Handles classroom business logic
      */

    public class ClassroomService : IClassroomService
    {
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
}
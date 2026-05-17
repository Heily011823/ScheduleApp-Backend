using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleApp.Application.Interfaces
{
    /*
     * Author: Salome Carmona
     * Feature: Classroom CRUD
     * Description: Repository interface for classroom operations using Guid
     */
    public interface IClassroomRepository
    {
        Task<List<Classroom>> GetAllAsync();

        // CAMBIADO: 'int id' por 'Guid id'
        Task<Classroom?> GetByIdAsync(Guid id);

        Task CreateAsync(Classroom classroom);

        Task UpdateAsync(Classroom classroom);

        // CAMBIADO: 'int id' por 'Guid id'
        Task DeleteAsync(Guid id);

        /// <summary>Busca un aula por su código único.</summary>
        Task<Classroom?> GetByCodeAsync(string code);

      
        Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);
    }
}
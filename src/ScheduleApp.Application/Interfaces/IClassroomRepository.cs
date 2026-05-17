using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Interfaces
{
    /*
     * Author: Salome Carmona
     * Feature: Classroom CRUD
     * Description: Repository interface for classroom operations
     */

    public interface IClassroomRepository
    {
        Task<List<Classroom>> GetAllAsync();

        Task<Classroom?> GetByIdAsync(int id);

        Task CreateAsync(Classroom classroom);

        Task UpdateAsync(Classroom classroom);

        Task DeleteAsync(int id);

        /// <summary>Busca un aula por su código único.</summary>
        Task<Classroom?> GetByCodeAsync(string code);

        /// <summary>Cambia el estado activo/inactivo de un aula en BD.</summary>
        Task<Classroom?> ChangeStatusAsync(int id, bool isActive);
    }
}

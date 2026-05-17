using ScheduleApp.Domain.Entities;
<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
=======
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c

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

<<<<<<< HEAD
        // CAMBIADO: 'int id' por 'Guid id'
        Task<Classroom?> GetByIdAsync(Guid id);
=======
        Task<Classroom?> GetByIdAsync(Guid id);        
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c

        Task CreateAsync(Classroom classroom);

        Task UpdateAsync(Classroom classroom);

<<<<<<< HEAD
        // CAMBIADO: 'int id' por 'Guid id'
        Task DeleteAsync(Guid id);
=======
        Task DeleteAsync(Guid id);                     
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c

        /// <summary>Busca un aula por su código único.</summary>
        Task<Classroom?> GetByCodeAsync(string code);

<<<<<<< HEAD
      
        Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);
=======
        /// <summary>Cambia el estado activo/inactivo de un aula en BD.</summary>
        Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);  // ← int → Guid
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
    }
}
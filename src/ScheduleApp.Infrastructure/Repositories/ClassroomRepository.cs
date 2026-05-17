using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ScheduleApp.Infrastructure.Repositories
{
    /*
      * Author: Salome Carmona
      * Feature: Classroom CRUD
      * Description: Handles classroom database operations
      */

    public class ClassroomRepository : IClassroomRepository
    {
        private readonly AppDbContext _context;

        public ClassroomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Classroom>> GetAllAsync()
        {
            return await _context.Classrooms.ToListAsync();
        }

        public async Task<Classroom?> GetByIdAsync(int id)
        {
            return await _context.Classrooms.FindAsync(id);
        }

        /// <summary>
        /// Crea un aula validando que el código sea único.
        /// Criterio: si el código ya existe → error.
        /// Criterio: si el código es único → permite guardar.
        /// </summary>
        /// Autor: Mateo Quintero
        /// Version: 0.1
        /// Rama: 84-validar-código-único-de-aula
        public async Task CreateAsync(Classroom classroom)
        {
            await _context.Classrooms.AddAsync(classroom);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un aula validando que el código sea único.
        /// Criterio: si el código ya existe en otra aula → error.
        /// Criterio: si el código es único → permite guardar.
        /// </summary>
        /// Autor: Mateo Quintero
        /// Version: 0.1
        /// Rama: 84-validar-código-único-de-aula
        public async Task UpdateAsync(Classroom classroom)
        {
            _context.Classrooms.Update(classroom);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);

            if (classroom != null)
            {
                _context.Classrooms.Remove(classroom);

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Busca un aula por su código único (case-insensitive).
        /// </summary>
        /// Autor: Mateo Quintero
        /// Rama: 84-validar-código-único-de-aula
        public async Task<Classroom?> GetByCodeAsync(string code)
        {
            return await _context.Classrooms
                .FirstOrDefaultAsync(c =>
                    c.Code.ToLower() == code.ToLower().Trim());
        }


        /// <summary>
        /// Cambia el estado activo/inactivo de un aula en BD.
        /// </summary>
        /// Autor: Mateo Quintero
        /// Rama: 85-implementar-cambio-de-estado-de-aula
        public async Task<Classroom?> ChangeStatusAsync(int id, bool isActive)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom is null) return null;

            classroom.IsActive = isActive;
            classroom.UpdatedAt = DateTime.UtcNow;

            _context.Classrooms.Update(classroom);
            await _context.SaveChangesAsync();
            return classroom;
        }
    }
}

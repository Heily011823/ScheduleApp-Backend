<<<<<<< HEAD
﻿// src/ScheduleApp.Infrastructure/Repositories/ClassroomRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
=======
﻿using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;


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

    // CORREGIDO: Cambiado de int a Guid
    public async Task<Classroom?> GetByIdAsync(Guid id)
    {
        return await _context.Classrooms.FindAsync(id);
    }

    public async Task<Classroom?> GetByCodeAsync(string code)
    {
        return await _context.Classrooms.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task CreateAsync(Classroom classroom)
    {
        // Si el Guid viene vacío, lo generamos antes de guardar
        if (classroom.Id == Guid.Empty)
        {
            classroom.Id = Guid.NewGuid();
        }

        await _context.Classrooms.AddAsync(classroom);
        await _context.SaveChangesAsync();
    }

<<<<<<< HEAD
    public async Task UpdateAsync(Classroom classroom)
    {
        _context.Classrooms.Update(classroom);
        await _context.SaveChangesAsync();
    }
=======
        public async Task<Classroom?> GetByIdAsync(Guid id)   // ← int → Guid
        {
            return await _context.Classrooms.FindAsync(id);
        }
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c

 
    public async Task DeleteAsync(Guid id)
    {
        var classroom = await GetByIdAsync(id);
        if (classroom != null)
        {
            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
        }
<<<<<<< HEAD
    }

  
    public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
    {
        var classroom = await GetByIdAsync(id);
        if (classroom == null) return null;

        classroom.IsActive = isActive;
        _context.Classrooms.Update(classroom);
        await _context.SaveChangesAsync();

        return classroom;
    }
=======

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

        public async Task DeleteAsync(Guid id)   // ← int → Guid
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
        public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)   // ← int → Guid
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
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
}
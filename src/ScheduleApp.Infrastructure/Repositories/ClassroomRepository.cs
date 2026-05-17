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

        public async Task CreateAsync(Classroom classroom)
        {
            await _context.Classrooms.AddAsync(classroom);

            await _context.SaveChangesAsync();
        }

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
    }
}

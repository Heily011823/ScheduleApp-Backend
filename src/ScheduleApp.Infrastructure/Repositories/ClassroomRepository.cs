using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleApp.Infrastructure.Repositories
{
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

        public async Task<Classroom?> GetByIdAsync(Guid id)
        {
            return await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Classroom?> GetByCodeAsync(string code)
        {
            return await _context.Classrooms
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower().Trim());
        }

        public async Task CreateAsync(Classroom classroom)
        {
            if (classroom.Id == Guid.Empty)
                classroom.Id = Guid.NewGuid();

            await _context.Classrooms.AddAsync(classroom);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Classroom classroom)
        {
            var existing = await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == classroom.Id);
            if (existing != null)
            {
                // Actualiza propiedad por propiedad
                existing.Code = classroom.Code;
                existing.Name = classroom.Name;
                existing.Building = classroom.Building;
                existing.Floor = classroom.Floor;
                existing.Capacity = classroom.Capacity;
                existing.Type = classroom.Type;
                existing.IsActive = classroom.IsActive;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == id);
            if (classroom != null)
            {
                _context.Classrooms.Remove(classroom);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive)
        {
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == id);
            if (classroom != null)
            {
                classroom.IsActive = isActive;
                await _context.SaveChangesAsync();
            }
            return classroom;
        }
    }
}
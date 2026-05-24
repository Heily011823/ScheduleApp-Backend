using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio de docentes usando Entity Framework Core.
    /// Implementa búsquedas, persistencia y carga de relaciones normalizadas.
    /// </summary>
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAvailableTeachersAsync()
        {
            return await _context.Teachers
                .Where(t => t.IsActive == true)
                .ToListAsync();
        }

        /// <summary>
        /// Busca docentes con filtros opcionales e incluye sus relaciones (Eager Loading).
        /// </summary>
        public async Task<IEnumerable<Teacher>> SearchAsync(string? name, bool? isActive)
        {
            var query = _context.Teachers
                .Include(t => t.Availabilities)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName)
                        .ToLower()
                        .Contains(name.ToLower()));
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            return await query
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .ToListAsync();
        }

        /// <summary>
        /// Busca un docente por su ID incluyendo su disponibilidad, especialidades y materias.
        /// </summary>
        public async Task<Teacher?> GetByIdAsync(Guid id)
        {
            // ✅ CORREGIDO: usar TeacherSpecialties en lugar de Specialty
            return await _context.Teachers
                .Include(t => t.Availabilities)
                .Include(t => t.TeacherSpecialties)
                    .ThenInclude(ts => ts.Specialty)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Teacher?> GetByEmailAsync(string email)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower().Trim());
        }

        public async Task<Teacher?> GetByIdentityDocumentAsync(string identityDocument)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.IdentityDocument == identityDocument.Trim());
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Teacher>> QuickSearchAsync(string term, int limit)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return new List<Teacher>();

            var searchTerm = term.Trim().ToLower();

            var query = _context.Teachers
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.IsActive == true)
                .Where(t =>
                    t.IdentityDocument.ToLower().Contains(searchTerm) ||
                    (t.FirstName + " " + t.LastName).ToLower().Contains(searchTerm) ||
                    t.Email.ToLower().Contains(searchTerm));

            var results = await query.Take(limit).ToListAsync();
            return results;
        }

        /// <summary>
        /// Búsqueda avanzada con múltiples filtros optimizada
        /// </summary>
        public async Task<IEnumerable<Teacher>> SearchAdvancedAsync(
            string? document,
            string? name,
            string? email,
            bool? isActive)
        {
            // ✅ CORREGIDO: usar TeacherSpecialties en lugar de Specialty
            var query = _context.Teachers
                .Include(t => t.Availabilities)
                .Include(t => t.TeacherSpecialties)
                    .ThenInclude(ts => ts.Specialty)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(document))
            {
                query = query.Where(t => t.IdentityDocument.Contains(document.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchTerm = name.Trim().ToLower();
                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName).ToLower().Contains(searchTerm) ||
                    t.FirstName.ToLower().Contains(searchTerm) ||
                    t.LastName.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(t => t.Email.ToLower().Contains(email.Trim().ToLower()));
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync()
        {
            return await _context.Specialties
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<bool> SpecialtyExistsAsync(string name)
        {
            return await _context.Specialties
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task AddSpecialtyAsync(Specialty specialty)
        {
            await _context.Specialties.AddAsync(specialty);
            await _context.SaveChangesAsync();
        }
    }
}
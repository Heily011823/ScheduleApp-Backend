using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
using ScheduleApp.Application.DTOs;

// Ruta recomendada: src/ScheduleApp.Infrastructure/Repositories/TeacherRepository.cs
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

        /*
         * Author: Salome Carmona
         * Feature: Available Teachers
         * Description: Returns active and available teachers
         */
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
            // CORREGIDO: Incluimos las relaciones atómicas para que el Service pueda mapear las especialidades y horas
            var query = _context.Teachers
                .Include(t => t.Availabilities)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject) // Carga la materia asignada en la tabla intermedia
                .AsQueryable();

            // Filtro por nombre completo (FirstName + LastName) optimizado para base de datos
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName)
                        .ToLower()
                        .Contains(name.ToLower()));
            }

            // Filtro por estado (Activo/Inactivo)
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
        /// Busca un docente por su ID incluyendo su disponibilidad y materias.
        /// </summary>
        public async Task<Teacher?> GetByIdAsync(Guid id)
        {


            // CORREGIDO: FindAsync no soporta .Include(), por lo que mutamos a FirstOrDefaultAsync
            return await _context.Teachers
                .Include(t => t.Availabilities)
                .Include(t => t.Specialty)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Busca un docente por correo electrónico.
        /// </summary>
        public async Task<Teacher?> GetByEmailAsync(string email)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower().Trim());
        }

        /// <summary>
        /// Busca un docente por documento de identidad.
        /// </summary>
        public async Task<Teacher?> GetByIdentityDocumentAsync(string identityDocument)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.IdentityDocument == identityDocument.Trim());
        }

        /// <summary>
        /// Guarda un nuevo docente y confirma la transacción de forma asíncrona.
        /// </summary>
        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un docente existente y vuelca los cambios a la base de datos.
        /// </summary>
        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Búsqueda rápida optimizada para autocompletado
        /// </summary>
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

            // Log para depuración
            var sql = query.ToQueryString();
            Console.WriteLine($"SQL generado: {sql}");

            var results = await query.Take(limit).ToListAsync();
            Console.WriteLine($"Resultados encontrados: {results.Count}");

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
            var query = _context.Teachers
                .Include(t => t.Availabilities)
                .Include(t => t.Specialty)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .AsQueryable();

            // 🔍 Búsqueda por documento
            if (!string.IsNullOrWhiteSpace(document))
            {
                query = query.Where(t => t.IdentityDocument.Contains(document.Trim()));
                Console.WriteLine($"Filtrando por documento: {document}"); // Para depuración
            }

            // 🔍 Búsqueda por nombre completo
            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchTerm = name.Trim().ToLower();
                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName).ToLower().Contains(searchTerm) ||
                    t.FirstName.ToLower().Contains(searchTerm) ||
                    t.LastName.ToLower().Contains(searchTerm));
                Console.WriteLine($"Filtrando por nombre: {name}");
            }

            // 🔍 Búsqueda por correo electrónico
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(t => t.Email.ToLower().Contains(email.Trim().ToLower()));
                Console.WriteLine($"Filtrando por email: {email}");
            }

            // Filtro por estado
            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
                Console.WriteLine($"Filtrando por estado activo: {isActive.Value}");
            }

            var results = await query.ToListAsync();
            Console.WriteLine($"Total resultados encontrados: {results.Count}");

            return results;
        }


        /// <summary>
        /// Obtiene todas las especialidades activas
        /// </summary>
        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync()
        {
            return await _context.Specialties
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si existe una especialidad por nombre
        /// </summary>
        public async Task<bool> SpecialtyExistsAsync(string name)
        {
            return await _context.Specialties
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Agrega una nueva especialidad
        /// </summary>
        public async Task AddSpecialtyAsync(Specialty specialty)
        {
            await _context.Specialties.AddAsync(specialty);
            await _context.SaveChangesAsync();
        }

        public Guid? SpecialtyId { get; set; }
        public string? SpecialtyName { get; set; }

    }
}
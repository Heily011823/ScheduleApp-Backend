using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

// Ruta original: src/ScheduleApp.Application/Services/TeacherService.cs
namespace ScheduleApp.Application.Services
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de docentes (Modelo Normalizado).
    /// </summary>
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISubjectRepository _subjectRepository;

        public TeacherService(ITeacherRepository teacherRepository, ISubjectRepository subjectRepository)
        {
            _teacherRepository = teacherRepository;
            _subjectRepository = subjectRepository;
        }

        /*
         * Author: Salome Carmona
         * Feature: Available Teachers
         * Description: Handles available teachers business logic
         */
        public async Task<IEnumerable<Teacher>> GetAvailableTeachersAsync()
        {
            return await _teacherRepository.GetAvailableTeachersAsync();
        }

        /// <summary>
        /// Realiza la búsqueda de docentes aplicando filtros relacionales en la base de datos.
        /// </summary>
        public async Task<IEnumerable<TeacherResponseDto>> SearchAsync(string? name, string? academicProgram, string? status)
        {
            bool? isActiveFilter = null;

            // CORREGIDO: Traduce de manera robusta los estados de la UI ("Activo" / "Inactivo") 
            // o de la API ("active" / "inactive") al valor booleano que requiere el repositorio de EF Core.
            if (!string.IsNullOrEmpty(status) && !status.Equals("Estado", StringComparison.OrdinalIgnoreCase))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("Activo", StringComparison.OrdinalIgnoreCase))
                {
                    isActiveFilter = true;
                }
                else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase) ||
                         status.Equals("Inactivo", StringComparison.OrdinalIgnoreCase))
                {
                    isActiveFilter = false;
                }
            }

            // Buscamos los docentes en el repositorio aplicando los filtros base
            var teachers = await _teacherRepository.SearchAsync(name, isActiveFilter);

            // Filtro por programa académico integrado mediante las tablas relacionales normalizadas
            if (!string.IsNullOrEmpty(academicProgram))
            {
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                    ts.ContractType.Contains(academicProgram, StringComparison.OrdinalIgnoreCase)));
            }

            return teachers.Select(t => MapToResponseDto(t));
        }

        /// <summary>
        /// Obtiene un docente específico por su identificador único.
        /// </summary>
        public async Task<TeacherResponseDto?> GetByIdAsync(Guid id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return null;

            return MapToResponseDto(teacher);
        }

        /// <summary>
        /// Obtiene un docente por su documento de identidad.
        /// </summary>
        public async Task<TeacherResponseDto?> GetByIdentityDocumentAsync(string document)
        {
            var teacher = await _teacherRepository.GetByIdentityDocumentAsync(document);
            if (teacher == null) return null;

            return MapToResponseDto(teacher);
        }

        /// <summary>
        /// Obtiene un docente por su correo electrónico.
        /// </summary>
        public async Task<TeacherResponseDto?> GetByEmailAsync(string email)
        {
            var teacher = await _teacherRepository.GetByEmailAsync(email);
            if (teacher == null) return null;

            return MapToResponseDto(teacher);
        }

        /// <summary>
        /// Obtiene todos los docentes activos (útiles para asignaciones y combos).
        /// </summary>
        public async Task<IEnumerable<TeacherResponseDto>> GetActiveTeachersAsync()
        {
            var teachers = await _teacherRepository.GetAvailableTeachersAsync();
            return teachers.Select(t => MapToResponseDto(t));
        }

        /// <summary>
        /// Búsqueda avanzada con todos los filtros requeridos por la historia #103
        /// </summary>
        public async Task<IEnumerable<TeacherResponseDto>> SearchAdvancedAsync(
            string? document,
            string? name,
            string? email,
            string? academicProgram,
            string? status)
        {
            // Convertir status a booleano
            bool? isActiveFilter = null;
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("Activo", StringComparison.OrdinalIgnoreCase))
                    isActiveFilter = true;
                else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase) ||
                         status.Equals("Inactivo", StringComparison.OrdinalIgnoreCase))
                    isActiveFilter = false;
            }

            // Búsqueda en repositorio con los filtros principales
            var teachers = await _teacherRepository.SearchAdvancedAsync(
                document, name, email, isActiveFilter);

            // Filtro adicional por programa académico (si existe)
            if (!string.IsNullOrEmpty(academicProgram))
            {
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                    ts.ContractType.Contains(academicProgram, StringComparison.OrdinalIgnoreCase) ||
                    (ts.Subject != null && ts.Subject.Name.Contains(academicProgram, StringComparison.OrdinalIgnoreCase))));
            }

            return teachers.Select(t => MapToResponseDto(t));
        }

        /// <summary>
        /// Búsqueda avanzada con paginación para mejorar el rendimiento en listados grandes.
        /// </summary>
        public async Task<(IEnumerable<TeacherResponseDto> Teachers, int TotalCount)> SearchAdvancedWithPaginationAsync(
            string? document,
            string? name,
            string? email,
            string? academicProgram,
            string? contractType,
            string? status,
            int page,
            int pageSize)
        {
            // Convertir status a booleano
            bool? isActiveFilter = null;
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("Activo", StringComparison.OrdinalIgnoreCase))
                    isActiveFilter = true;
                else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase) ||
                         status.Equals("Inactivo", StringComparison.OrdinalIgnoreCase))
                    isActiveFilter = false;
            }

            // Búsqueda en repositorio con los filtros principales
            var teachers = await _teacherRepository.SearchAdvancedAsync(
                document, name, email, isActiveFilter);

            // Filtro adicional por programa académico (si existe)
            if (!string.IsNullOrEmpty(academicProgram))
            {
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                    ts.ContractType.Contains(academicProgram, StringComparison.OrdinalIgnoreCase) ||
                    (ts.Subject != null && ts.Subject.Name.Contains(academicProgram, StringComparison.OrdinalIgnoreCase))));
            }

            // Filtro por tipo de contrato
            if (!string.IsNullOrEmpty(contractType))
            {
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                    ts.ContractType.Equals(contractType, StringComparison.OrdinalIgnoreCase)));
            }

            // Convertir a lista para contar y paginar
            var teacherList = teachers.ToList();
            var totalCount = teacherList.Count;

            // Aplicar paginación
            var paginatedTeachers = teacherList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => MapToResponseDto(t));

            return (paginatedTeachers, totalCount);
        }

        /// <summary>
        /// Búsqueda rápida para autocompletado (sugerencias en tiempo real).
        /// </summary>
        public async Task<IEnumerable<object>> QuickSearchAsync(string term, int limit)
        {
            // Validar que el término tenga al menos 2 caracteres
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                Console.WriteLine($"Término de búsqueda muy corto: '{term}'");
                return new List<object>();
            }

            Console.WriteLine($"Buscando docentes con término: '{term}', límite: {limit}");

            // Llamar al repositorio con búsqueda en documento, nombre y email
            var teachers = await _teacherRepository.SearchAdvancedAsync(
                document: term,  // Buscar en documento
                name: term,      // Buscar en nombre
                email: term,     // Buscar en email
                isActive: true); // Solo docentes activos

            Console.WriteLine($"Docentes encontrados (antes de mapeo): {teachers.Count()}");

            // Mostrar los primeros 5 resultados para depuración
            foreach (var t in teachers.Take(5))
            {
                Console.WriteLine($"- {t.FirstName} {t.LastName} | Doc: {t.IdentityDocument} | Email: {t.Email}");
            }

            // Mapear resultados
            var suggestions = teachers
                .Take(limit)
                .Select(t => new
                {
                    id = t.Id,
                    fullName = $"{t.FirstName} {t.LastName}",
                    document = t.IdentityDocument,
                    email = t.Email,
                    specialty = t.TeacherSubjects?.FirstOrDefault()?.Subject?.Name ?? "General"
                });

            return suggestions;
        }

        /// <summary>
        /// Obtiene estadísticas agregadas de docentes para el dashboard.
        /// </summary>
        public async Task<object> GetStatisticsAsync()
        {
            var allTeachers = await _teacherRepository.SearchAdvancedAsync(null, null, null, null);
            var activeTeachers = allTeachers.Where(t => t.IsActive);
            var inactiveTeachers = allTeachers.Where(t => !t.IsActive);

            // Estadísticas por tipo de contrato
            var contractTypeStats = allTeachers
                .SelectMany(t => t.TeacherSubjects)
                .GroupBy(ts => ts.ContractType)
                .ToDictionary(g => g.Key, g => g.Count());

            // Total de horas de enseñanza
            var totalTeachingHours = allTeachers
                .SelectMany(t => t.Availabilities)
                .Sum(a => a.MaxTeachingHours);

            return new
            {
                total = allTeachers.Count(),
                active = activeTeachers.Count(),
                inactive = inactiveTeachers.Count(),
                byContractType = contractTypeStats,
                totalTeachingHours = totalTeachingHours,
                averageHoursPerTeacher = allTeachers.Any() ? totalTeachingHours / allTeachers.Count() : 0
            };
        }

        /// <summary>
        /// Crea un nuevo docente de forma atómica en la base de datos junto a sus entidades hijas.
        /// </summary>
        public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
        {
            // Validación de duplicados por correo electrónico
            var existingEmail = await _teacherRepository.GetByEmailAsync(dto.Email);
            if (existingEmail != null)
                throw new InvalidOperationException("El correo electrónico ya está registrado por otro docente.");

            // Validación de duplicados por documento de identidad
            var existingDoc = await _teacherRepository.GetByIdentityDocumentAsync(dto.IdentityDocument);
            if (existingDoc != null)
                throw new InvalidOperationException("El documento de identidad ya está registrado.");

            var teacherId = Guid.NewGuid();

            // 1. Instanciamos la entidad principal de Dominio
            var teacher = new Teacher
            {
                Id = teacherId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                IdentityDocument = dto.IdentityDocument,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 2. NORMALIZACIÓN: Mapeamos la disponibilidad (1 a Muchos) de forma atómica
            teacher.Availabilities.Add(new TeacherAvailability
            {
                Id = Guid.NewGuid(),
                TeacherId = teacherId,
                Day = DayOfWeek.Monday, // Por defecto asignamos días hábiles base o extendibles según requiera la lógica
                StartTime = TimeSpan.FromHours(7),  // 07:00 AM asignado por defecto
                EndTime = TimeSpan.FromHours(18),   // 06:00 PM asignado por defecto
                MaxTeachingHours = dto.TeachingHours // Guardamos las horas reales numéricas de forma limpia
            });

            // 3. NORMALIZACIÓN: Mapeamos la relación N:M con materias mediante la entidad intermedia
            var (subjectsList, _) = await _subjectRepository.SearchAsync(dto.Specialties, null, true, 1, 1);
            var subjectBaseId = subjectsList.FirstOrDefault()?.Id ?? Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            teacher.TeacherSubjects.Add(new TeacherSubject
            {
                TeacherId = teacherId,
                SubjectId = subjectBaseId,
                ContractType = dto.ContractType // Guardamos la especialidad/programa de forma estructurada
            });

            await _teacherRepository.AddAsync(teacher);

            return MapToResponseDto(teacher);
        }

        /// <summary>
        /// Actualiza la información de un docente existente validando restricciones de unicidad.
        /// </summary>
        /// Autor: Mateo Quintero
        /// Version: 0.1
        /// Rama: 97-validar-documento-único-de-docente
        public async Task<TeacherResponseDto?> UpdateAsync(Guid id, UpdateTeacherDto dto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return null;

            // ── Validación #97: documento único al actualizar ────────────────────
            if (teacher.IdentityDocument != dto.IdentityDocument.Trim())
            {
                var existingDoc = await _teacherRepository
                    .GetByIdentityDocumentAsync(dto.IdentityDocument.Trim());

                if (existingDoc is not null && existingDoc.Id != id)
                    throw new InvalidOperationException(
                        $"Ya existe otro docente registrado con el documento '{dto.IdentityDocument}'.");
            }

            // ── Validación #98: correo único al actualizar ───────────────────────
            if (teacher.Email != dto.Email.ToLower().Trim())
            {
                var existingEmail = await _teacherRepository
                    .GetByEmailAsync(dto.Email.ToLower().Trim());

                if (existingEmail is not null && existingEmail.Id != id)
                    throw new InvalidOperationException(
                        $"Ya existe otro docente registrado con el correo '{dto.Email}'.");
            }

            // Actualizamos los campos directos en el contexto
            teacher.FirstName = dto.FirstName;
            teacher.LastName = dto.LastName;
            teacher.Email = dto.Email.ToLower().Trim();
            teacher.IdentityDocument = dto.IdentityDocument.Trim();
            teacher.PhoneNumber = dto.PhoneNumber;
            teacher.IsActive = dto.IsActive;
            teacher.UpdatedAt = DateTime.UtcNow;

            // Actualización de la entidad de disponibilidad hija
            var availability = teacher.Availabilities.FirstOrDefault();
            if (availability != null)
                availability.MaxTeachingHours = dto.TeachingHours;

            // Actualización de la relación intermedia con asignación de materias
            var teacherSubject = teacher.TeacherSubjects.FirstOrDefault();
            if (teacherSubject != null)
                teacherSubject.ContractType = dto.ContractType;

            await _teacherRepository.UpdateAsync(teacher);
            return MapToResponseDto(teacher);
        }

        /// <summary>
        /// Búsqueda avanzada con todos los filtros requeridos por la historia #103
        /// </summary>
        public async Task<IEnumerable<TeacherResponseDto>> SearchAdvancedAsync(
            string? document,
            string? name,
            string? email,
            string? academicProgram,
            string? contractType,  // ← Agrega este parámetro
            string? status)
        {
            // Convertir status a booleano
            bool? isActiveFilter = null;
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("Activo", StringComparison.OrdinalIgnoreCase))
                    isActiveFilter = true;
                else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase) ||
                         status.Equals("Inactivo", StringComparison.OrdinalIgnoreCase))
                    isActiveFilter = false;
            }

            // Búsqueda en repositorio con los filtros principales
            var teachers = await _teacherRepository.SearchAdvancedAsync(
                document, name, email, isActiveFilter);

            // Filtro adicional por programa académico (si existe)
            if (!string.IsNullOrEmpty(academicProgram))
            {
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                    ts.ContractType.Contains(academicProgram, StringComparison.OrdinalIgnoreCase) ||
                    (ts.Subject != null && ts.Subject.Name.Contains(academicProgram, StringComparison.OrdinalIgnoreCase))));
            }

            // Filtro por tipo de contrato (NUEVO)
            if (!string.IsNullOrEmpty(contractType))
            {
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                    ts.ContractType.Equals(contractType, StringComparison.OrdinalIgnoreCase)));
            }

            return teachers.Select(t => MapToResponseDto(t));
        }


        /// <summary>
        /// Ejecuta una desactivación lógica (Soft Delete) del docente.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return false;

            teacher.IsActive = false;
            teacher.UpdatedAt = DateTime.UtcNow;

            await _teacherRepository.UpdateAsync(teacher);
            return true;
        }

        /// <summary>
        /// Cambia explícitamente el estado del docente habilitándolo o deshabilitándolo en las asignaciones.
        /// </summary>
        public async Task<TeacherResponseDto?> ChangeStatusAsync(Guid id, bool isActive)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher is null) return null;

            teacher.IsActive = isActive;
            teacher.UpdatedAt = DateTime.UtcNow;

            await _teacherRepository.UpdateAsync(teacher);
            return MapToResponseDto(teacher);
        }

        /// <summary>
        /// Mapea de forma segura las entidades relacionales complejas hacia el DTO plano de salida.
        /// </summary>
        private static TeacherResponseDto MapToResponseDto(Teacher teacher)
        {
            var mainSubject = teacher.TeacherSubjects?.FirstOrDefault();
            var mainAvailability = teacher.Availabilities?.FirstOrDefault();

            return new TeacherResponseDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                IdentityDocument = teacher.IdentityDocument,
                PhoneNumber = teacher.PhoneNumber,
                IsActive = teacher.IsActive,
                CreatedAt = teacher.CreatedAt,
                UpdatedAt = teacher.UpdatedAt,
                // ======================================================
                // RELACIÓN MANY TO MANY
                // ======================================================
                ContractType = mainSubject?.ContractType ?? "No asignado",
                // ======================================================
                // ESPECIALIDAD
                // ======================================================
                Specialties = mainSubject?.Subject?.Name ?? "General",
                // ======================================================
                // HORAS
                // ======================================================
                TeachingHours = mainAvailability?.MaxTeachingHours ?? 0
            };
        }
    }
}
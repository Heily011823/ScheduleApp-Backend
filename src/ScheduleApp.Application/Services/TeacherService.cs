using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

// src/ScheduleApp.Application/Services/TeacherService.cs
namespace ScheduleApp.Application.Services;

/// <summary>
/// Implementación de la lógica de negocio para la gestión de docentes (Modelo Normalizado).
/// </summary>
public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ISubjectRepository _subjectRepository; // Agregado para validar la existencia de materias si es necesario

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
    public async Task<IEnumerable<TeacherResponseDto>> SearchAsync(string? name, string? academicProgram, string? status)
    {
        bool? isActiveFilter = null;
        if (!string.IsNullOrEmpty(status))
        {
            isActiveFilter = status.Equals("active", StringComparison.OrdinalIgnoreCase);
        }

        // Buscamos los docentes en el repositorio
        var teachers = await _teacherRepository.SearchAsync(name, isActiveFilter);

        // Filtro por programa académico integrado mediante las tablas relacionales normalizadas
        if (!string.IsNullOrEmpty(academicProgram))
        {
            teachers = teachers.Where(t => t.TeacherSubjects.Any(ts =>
                ts.ContractType.Contains(academicProgram, StringComparison.OrdinalIgnoreCase)));
        }

        return teachers.Select(t => MapToResponseDto(t));
    }

    public async Task<TeacherResponseDto?> GetByIdAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null) return null;

        return MapToResponseDto(teacher);
    }

    public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
    {
        // Validación de duplicados
        var existingEmail = await _teacherRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
            throw new InvalidOperationException("El correo electrónico ya está registrado por otro docente.");

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
        // Buscamos si existe una materia que coincida con la especialidad para asociar un ID real
        // LÍNEA CORREGIDA:
        var subjects = await _subjectRepository.SearchAsync(dto.Specialties, null, true);
        var subjectBaseId = subjects.FirstOrDefault()?.Id ?? Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // ID semilla o fallback

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
    /// Actualiza los datos de un docente existente.
    /// </summary>
    /// Autor: Mateo Quintero
    /// Version: 0.1
    /// Rama: 97-validar-documento-único-de-docente
    public async Task<TeacherResponseDto?> UpdateAsync(Guid id, UpdateTeacherDto dto)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null) return null;

        // ── Validación #97: documento único al actualizar ────────────────────
        // Criterio: si ya existe otro docente con el mismo documento, lanzar error.
        // Criterio: si el documento es único, permitir guardar.
        if (teacher.IdentityDocument != dto.IdentityDocument.Trim())
        {
            var existingDoc = await _teacherRepository
                .GetByIdentityDocumentAsync(dto.IdentityDocument.Trim());

            if (existingDoc is not null && existingDoc.Id != id)
                throw new InvalidOperationException(
                    $"Ya existe otro docente registrado con el documento '{dto.IdentityDocument}'.");
        }

        // ── Validación #98: correo único al actualizar ───────────────────────
        // Criterio: si ya existe otro docente con el mismo correo, lanzar error.
        // Criterio: si el correo es único, permitir guardar.
        if (teacher.Email != dto.Email.ToLower().Trim())
        {
            var existingEmail = await _teacherRepository
                .GetByEmailAsync(dto.Email.ToLower().Trim());

            if (existingEmail is not null && existingEmail.Id != id)
                throw new InvalidOperationException(
                    $"Ya existe otro docente registrado con el correo '{dto.Email}'.");
        }

        // Actualizamos los campos directos
        teacher.FirstName = dto.FirstName;
        teacher.LastName = dto.LastName;
        teacher.Email = dto.Email.ToLower().Trim();
        teacher.IdentityDocument = dto.IdentityDocument.Trim();
        teacher.PhoneNumber = dto.PhoneNumber;
        teacher.IsActive = dto.IsActive;
        teacher.UpdatedAt = DateTime.UtcNow;

        // Actualización de disponibilidad
        var availability = teacher.Availabilities.FirstOrDefault();
        if (availability != null)
            availability.MaxTeachingHours = dto.TeachingHours;

        // Actualización de la tabla intermedia
        var teacherSubject = teacher.TeacherSubjects.FirstOrDefault();
        if (teacherSubject != null)
            teacherSubject.ContractType = dto.ContractType;

        await _teacherRepository.UpdateAsync(teacher);
        return MapToResponseDto(teacher);
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null) return false;

        // Borrado lógico reglamentario
        teacher.IsActive = false;
        teacher.UpdatedAt = DateTime.UtcNow;

        await _teacherRepository.UpdateAsync(teacher);
        return true;
    }

    // <summary>
    /// Cambia el estado activo/inactivo de un docente.
    /// Criterio: si está activo y se desactiva, no aparece en asignaciones.
    /// Criterio: si está inactivo y se activa, vuelve a estar disponible.
    /// </summary>
    /// Autor: Mateo Quintero
    /// Version: 0.1
    /// Rama: 99-implementar-cambio-de-estado-de-docente
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
    /// Mapea de forma segura los objetos hijos relacionales hacia el DTO plano de salida.
    /// </summary>
    private static TeacherResponseDto MapToResponseDto(Teacher teacher)
    {
        var mainSubject = teacher.TeacherSubjects.FirstOrDefault();
        var mainAvailability = teacher.Availabilities.FirstOrDefault();

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

            // Extraemos los valores desde sus respectivas relaciones atómicas normalizadas
            ContractType = mainSubject != null ? mainSubject.ContractType : "No asignado",
            Specialties = mainSubject != null && mainSubject.Subject != null ? mainSubject.Subject.Name : "General",
            TeachingHours = mainAvailability != null ? mainAvailability.MaxTeachingHours : 0
        };
    }
}
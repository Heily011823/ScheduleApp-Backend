using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

// Ruta recomendada: src/ScheduleApp.Application/Services/TeacherService.cs
namespace ScheduleApp.Application.Services;

/// <summary>
/// Implementación de la lógica de negocio para la gestión de docentes.
/// </summary>
public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;

    public TeacherService(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task<IEnumerable<TeacherResponseDto>> SearchAsync(string? name, string? academicProgram, string? status)
    {
        bool? isActiveFilter = null;
        if (!string.IsNullOrEmpty(status))
        {
            isActiveFilter = status.Equals("active", StringComparison.OrdinalIgnoreCase);
        }

        // Buscamos en el repositorio usando los filtros correspondientes
        var teachers = await _teacherRepository.SearchAsync(name, isActiveFilter);

        // Si se especificó filtro de programa académico, filtramos en memoria
        if (!string.IsNullOrEmpty(academicProgram))
        {
            teachers = teachers.Where(t => t.AcademicProgram.Contains(academicProgram, StringComparison.OrdinalIgnoreCase));
        }

        // Proyectamos las entidades de Dominio a DTOs de respuesta
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
        // Validación de negocio básica: Evitar correos o documentos duplicados
        var existingEmail = await _teacherRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
            throw new InvalidOperationException("El correo electrónico ya está registrado por otro docente.");

        var existingDoc = await _teacherRepository.GetByIdentityDocumentAsync(dto.IdentityDocument);
        if (existingDoc != null)
            throw new InvalidOperationException("El documento de identidad ya está registrado.");

        // Mapeamos el DTO de creación a nuestra Entidad de Dominio real
        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IdentityDocument = dto.IdentityDocument,
            PhoneNumber = dto.PhoneNumber,
            // Guardamos las especialidades/contratos dentro de los campos de tu entidad de dominio
            AcademicProgram = dto.ContractType,
            PreferredSubject = dto.Specialties,
            Availability = $"Horas asignadas: {dto.TeachingHours}",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _teacherRepository.AddAsync(teacher);

        return MapToResponseDto(teacher);
    }

    public async Task<TeacherResponseDto?> UpdateAsync(Guid id, UpdateTeacherDto dto)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null) return null;

        // Actualizamos los campos de la entidad con los nuevos datos del DTO
        teacher.FirstName = dto.FirstName;
        teacher.LastName = dto.LastName;
        teacher.Email = dto.Email;
        teacher.IdentityDocument = dto.IdentityDocument;
        teacher.PhoneNumber = dto.PhoneNumber;
        teacher.PreferredSubject = dto.Specialties;
        teacher.AcademicProgram = dto.ContractType;
        teacher.Availability = $"Horas asignadas: {dto.TeachingHours}";
        teacher.IsActive = dto.IsActive;
        teacher.UpdatedAt = DateTime.UtcNow;

        await _teacherRepository.UpdateAsync(teacher);

        return MapToResponseDto(teacher);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null) return false;

        // Aplicamos un borrado lógico (cambiar estado a inactivo) en lugar de un borrado físico duro
        teacher.IsActive = false;
        teacher.UpdatedAt = DateTime.UtcNow;

        await _teacherRepository.UpdateAsync(teacher);
        return true;
    }

    /// <summary>
    /// Método privado auxiliar para mapear una entidad 'Teacher' a un 'TeacherResponseDto'.
    /// </summary>
    private static TeacherResponseDto MapToResponseDto(Teacher teacher)
    {
        return new TeacherResponseDto
        {
            Id = teacher.Id,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email,
            IdentityDocument = teacher.IdentityDocument,
            PhoneNumber = teacher.PhoneNumber,
            Specialties = teacher.PreferredSubject,
            ContractType = teacher.AcademicProgram,
            IsActive = teacher.IsActive,
            CreatedAt = teacher.CreatedAt,
            UpdatedAt = teacher.UpdatedAt,
            // Extraemos las horas del campo string de disponibilidad si es posible, por defecto 0
            TeachingHours = teacher.Availability.Contains("Horas asignadas:")
                ? int.Parse(string.Concat(teacher.Availability.Where(char.IsDigit)))
                : 0
        };
    }
}
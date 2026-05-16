using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Services/TeacherService.cs
namespace ScheduleApp.Application.Services;

using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

/// <summary>
/// Servicio con la lógica de negocio del módulo de docentes.
/// Gestiona CRUD completo con validaciones de unicidad.
/// </summary>
/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes
public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;

    public TeacherService(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    /// <summary>
    /// Retorna docentes filtrados por nombre, especialidad y/o estado.
    /// Si no se pasan filtros, retorna todos los docentes.
    /// </summary>
    public async Task<IEnumerable<TeacherResponseDto>> SearchAsync(
        string? name,
        string? specialty,
        bool? isActive)
    {
        var teachers = await _teacherRepository.SearchAsync(
            name, specialty, isActive);
        return teachers.Select(MapToDto);
    }

    /// <summary>
    /// Retorna un docente por su ID.
    /// Retorna null si no existe.
    /// </summary>
    public async Task<TeacherResponseDto?> GetByIdAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        return teacher is null ? null : MapToDto(teacher);
    }

    /// <summary>
    /// Crea un nuevo docente validando que email y documento sean únicos.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Si ya existe un docente con el mismo email o documento.
    /// </exception>
    public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
    {
        // Validar email único
        var existingByEmail = await _teacherRepository
            .GetByEmailAsync(dto.Email.ToLower().Trim());
        if (existingByEmail is not null)
            throw new InvalidOperationException(
                $"Ya existe un docente registrado con el email '{dto.Email}'.");

        // Validar documento único
        var existingByDoc = await _teacherRepository
            .GetByIdentityDocumentAsync(dto.IdentityDocument.Trim());
        if (existingByDoc is not null)
            throw new InvalidOperationException(
                $"Ya existe un docente registrado con el documento '{dto.IdentityDocument}'.");

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            Email = dto.Email.ToLower().Trim(),
            IdentityDocument = dto.IdentityDocument.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Specialty = dto.Specialty.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _teacherRepository.AddAsync(teacher);
        return MapToDto(teacher);
    }

    /// <summary>
    /// Actualiza los datos de un docente existente.
    /// Valida unicidad de email y documento si cambian.
    /// Retorna null si el docente no existe.
    /// </summary>
    public async Task<TeacherResponseDto?> UpdateAsync(Guid id, UpdateTeacherDto dto)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher is null) return null;

        // Validar email único si cambió
        var normalizedEmail = dto.Email.ToLower().Trim();
        if (teacher.Email != normalizedEmail)
        {
            var existing = await _teacherRepository.GetByEmailAsync(normalizedEmail);
            if (existing is not null && existing.Id != id)
                throw new InvalidOperationException(
                    $"Ya existe otro docente con el email '{dto.Email}'.");
        }

        // Validar documento único si cambió
        var normalizedDoc = dto.IdentityDocument.Trim();
        if (teacher.IdentityDocument != normalizedDoc)
        {
            var existing = await _teacherRepository
                .GetByIdentityDocumentAsync(normalizedDoc);
            if (existing is not null && existing.Id != id)
                throw new InvalidOperationException(
                    $"Ya existe otro docente con el documento '{dto.IdentityDocument}'.");
        }

        teacher.FullName = dto.FullName.Trim();
        teacher.Email = normalizedEmail;
        teacher.IdentityDocument = normalizedDoc;
        teacher.PhoneNumber = dto.PhoneNumber.Trim();
        teacher.Specialty = dto.Specialty.Trim();
        teacher.IsActive = dto.IsActive;
        teacher.UpdatedAt = DateTime.UtcNow;

        await _teacherRepository.UpdateAsync(teacher);
        return MapToDto(teacher);
    }

    /// <summary>
    /// Desactiva un docente (eliminación lógica).
    /// No se elimina físicamente para preservar historial de asignaciones.
    /// Retorna false si el docente no existe.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher is null) return false;

        teacher.IsActive = false;
        teacher.UpdatedAt = DateTime.UtcNow;

        await _teacherRepository.UpdateAsync(teacher);
        return true;
    }

    /// <summary>Mapea la entidad Teacher a su DTO de respuesta.</summary>
    private static TeacherResponseDto MapToDto(Teacher teacher) => new()
    {
        Id = teacher.Id,
        FullName = teacher.FullName,
        Email = teacher.Email,
        IdentityDocument = teacher.IdentityDocument,
        PhoneNumber = teacher.PhoneNumber,
        Specialty = teacher.Specialty,
        IsActive = teacher.IsActive,
        CreatedAt = teacher.CreatedAt,
        UpdatedAt = teacher.UpdatedAt
    };
}

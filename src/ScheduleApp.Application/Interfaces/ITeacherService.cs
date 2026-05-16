using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Interfaces/ITeacherService.cs
namespace ScheduleApp.Application.Interfaces;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes

using ScheduleApp.Application.DTOs;

/// <summary>
/// Contrato del servicio de docentes.
/// Define las operaciones de negocio disponibles para el coordinador.
/// </summary>
public interface ITeacherService
{
    /// <summary>Retorna docentes con filtros opcionales.</summary>
    Task<IEnumerable<TeacherResponseDto>> SearchAsync(
        string? name,
        string? specialty,
        bool? isActive);

    /// <summary>Retorna un docente por su ID.</summary>
    Task<TeacherResponseDto?> GetByIdAsync(Guid id);

    /// <summary>Crea un nuevo docente.</summary>
    Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto);

    /// <summary>Actualiza un docente existente.</summary>
    Task<TeacherResponseDto?> UpdateAsync(Guid id, UpdateTeacherDto dto);

    /// <summary>Desactiva un docente (eliminación lógica).</summary>
    Task<bool> DeleteAsync(Guid id);
}

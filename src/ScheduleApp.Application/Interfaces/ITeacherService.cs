using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Ruta recomendada: src/ScheduleApp.Application/Interfaces/ITeacherService.cs
namespace ScheduleApp.Application.Interfaces;

/// <summary>
/// Contrato del servicio de docentes.
/// Define las operaciones de negocio disponibles.
/// </summary>
public interface ITeacherService
{
    Task<IEnumerable<Teacher>> GetAvailableTeachersAsync();
    /// <summary>
    /// Retorna docentes con filtros opcionales.
    /// </summary>
    Task<IEnumerable<TeacherResponseDto>> SearchAsync(
        string? name,
        string? academicProgram,
        string? status);

    /// <summary>
    /// Retorna un docente por ID.
    /// </summary>
    Task<TeacherResponseDto?> GetByIdAsync(
        Guid id);

    /// <summary>
    /// Crea un nuevo docente.
    /// </summary>
    Task<TeacherResponseDto> CreateAsync(
        CreateTeacherDto dto);

    /// <summary>
    /// Actualiza un docente existente.
    /// </summary>
    Task<TeacherResponseDto?> UpdateAsync(
        Guid id,
        UpdateTeacherDto dto);

    /// <summary>
    /// Elimina lógicamente un docente.
    /// </summary>
    Task<bool> DeleteAsync(
        Guid id);

    /// <summary>
    /// Cambia el estado activo/inactivo de un docente.
    /// </summary>
    Task<TeacherResponseDto?> ChangeStatusAsync(Guid id, bool isActive);

}
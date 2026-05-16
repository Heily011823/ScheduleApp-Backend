using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/Interfaces/ITeacherRepository.cs
namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Domain.Entities;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes

/// <summary>
/// Contrato para acceso a datos de docentes.
/// Implementado en Infrastructure por TeacherRepository.
/// Usado por las ramas 41 (disponibilidad) y 42 (cruces).
/// </summary>


public interface ITeacherRepository
{
    /// <summary>Retorna todos los docentes con filtros opcionales.</summary>
    Task<IEnumerable<Teacher>> SearchAsync(
        string? name,
        string? specialty,
        bool? isActive);

    /// <summary>Busca un docente por su ID.</summary>
    Task<Teacher?> GetByIdAsync(Guid id);

    /// <summary>Busca un docente por su email.</summary>
    Task<Teacher?> GetByEmailAsync(string email);

    /// <summary>Busca un docente por su documento de identidad.</summary>
    Task<Teacher?> GetByIdentityDocumentAsync(string identityDocument);

    /// <summary>Guarda un nuevo docente en BD.</summary>
    Task AddAsync(Teacher teacher);

    /// <summary>Actualiza un docente existente en BD.</summary>
    Task UpdateAsync(Teacher teacher);
}
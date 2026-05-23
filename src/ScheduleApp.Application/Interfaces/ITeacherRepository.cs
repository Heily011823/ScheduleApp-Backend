using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Domain.Entities;

// Ruta recomendada: src/ScheduleApp.Application/Interfaces/ITeacherRepository.cs
namespace ScheduleApp.Application.Interfaces;

/// <summary>
/// Contrato para acceso a datos de docentes.
/// </summary>
public interface ITeacherRepository
{

    Task<IEnumerable<Teacher>> GetAvailableTeachersAsync();
    /// <summary>
    /// Retorna docentes con filtros opcionales.
    /// </summary>
    Task<IEnumerable<Teacher>> SearchAsync(
        string? name,
        bool? isActive);

    /// <summary>
    /// Busca un docente por ID.
    /// </summary>
    Task<Teacher?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca un docente por email.
    /// </summary>
    Task<Teacher?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca un docente por documento.
    /// </summary>
    Task<Teacher?> GetByIdentityDocumentAsync(string document);

    /// <summary>
    /// Guarda un docente.
    /// </summary>
    Task AddAsync(Teacher teacher);

    /// <summary>
    /// Actualiza un docente.
    /// </summary>
    Task UpdateAsync(Teacher teacher);


    /// <summary>
    /// Búsqueda rápida para autocompletado (optimizada)
    /// </summary>
    Task<IEnumerable<Teacher>> QuickSearchAsync(string term, int limit);

    /// <summary>
    /// Búsqueda avanzada con múltiples filtros optimizada
    /// </summary>
    Task<IEnumerable<Teacher>> SearchAdvancedAsync(
        string? document,
        string? name,
        string? email,
        bool? isActive);


    /// <summary>
    /// Obtiene todas las especialidades activas
    /// </summary>
    Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync();

    /// <summary>
    /// Verifica si existe una especialidad por nombre
    /// </summary>
    Task<bool> SpecialtyExistsAsync(string name);

    /// <summary>
    /// Agrega una nueva especialidad
    /// </summary>
    Task AddSpecialtyAsync(Specialty specialty);

}
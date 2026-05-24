using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;



/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: develop

/// <summary>
/// Repositorio especializado para operaciones de base de datos de especialidades.
/// </summary>
public interface ISpecialtyRepository
{
    /// <summary>
    /// Obtiene todas las especialidades activas
    /// </summary>
    Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync();

    /// <summary>
    /// Obtiene una especialidad por su ID
    /// </summary>
    Task<Specialty?> GetSpecialtyByIdAsync(Guid id);

    /// <summary>
    /// Obtiene una especialidad por su nombre
    /// </summary>
    Task<Specialty?> GetSpecialtyByNameAsync(string name);

    /// <summary>
    /// Verifica si existe una especialidad por nombre
    /// </summary>
    Task<bool> SpecialtyExistsAsync(string name);

    /// <summary>
    /// Agrega una nueva especialidad
    /// </summary>
    Task AddSpecialtyAsync(Specialty specialty);

    /// <summary>
    /// Actualiza una especialidad existente
    /// </summary>
    Task UpdateSpecialtyAsync(Specialty specialty);

    /// <summary>
    /// Verifica si hay docentes asociados a una especialidad
    /// </summary>
    Task<bool> HasTeachersWithSpecialtyAsync(Guid specialtyId);
}
// src/ScheduleApp.Application/Interfaces/IClassroomRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

/*
 * Author: Salome Carmona
 * Feature: Classroom CRUD
 * Description: Repository interface for classroom operations using Guid identifiers
 */
public interface IClassroomRepository
{
    /// <summary>Obtiene la lista completa de aulas.</summary>
    Task<List<Classroom>> GetAllAsync();

    /// <summary>Busca un aula específica por su identificador único Guid.</summary>
    Task<Classroom?> GetByIdAsync(Guid id);

    /// <summary>Registra una nueva aula en el sistema.</summary>
    Task CreateAsync(Classroom classroom);

    /// <summary>Actualiza las propiedades de un aula existente.</summary>
    Task UpdateAsync(Classroom classroom);

    /// <summary>Elimina un aula del sistema por su Guid.</summary>
    Task DeleteAsync(Guid id);

    /// <summary>Busca un aula por su código único institucional.</summary>
    Task<Classroom?> GetByCodeAsync(string code);

    /// <summary>Cambia el estado activo/inactivo de un aula en la base de datos.</summary>
    Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);
}
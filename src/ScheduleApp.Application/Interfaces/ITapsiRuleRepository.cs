using System;
using System.Collections.Generic;
using System.Text;

// ScheduleApp.Application/Interfaces/ITapsiRuleRepository.cs
namespace ScheduleApp.Application.Interfaces;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi


using ScheduleApp.Domain.Entities;

/// <summary>
/// Contrato del repositorio para gestión de reglas TAPSI en base de datos.
/// </summary>
public interface ITapsiRuleRepository
{
    /// <summary>Retorna todas las reglas TAPSI registradas.</summary>
    Task<IEnumerable<TapsiRule>> GetAllAsync();

    /// <summary>Retorna solo las reglas TAPSI activas.</summary>
    Task<IEnumerable<TapsiRule>> GetActiveAsync();

    /// <summary>Busca una regla por su ID.</summary>
    Task<TapsiRule?> GetByIdAsync(Guid id);

    /// <summary>Guarda una nueva regla.</summary>
    Task AddAsync(TapsiRule rule);

    /// <summary>Actualiza una regla existente.</summary>
    Task UpdateAsync(TapsiRule rule);

    /// <summary>Elimina una regla por su ID.</summary>
    Task DeleteAsync(Guid id);
}
// src/ScheduleApp.Application/Interfaces/IClassroomService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

/*
 * Author: Team ScheduleApp
 * Description: Application service interface for managing classroom business logic with Guid
 */
public interface IClassroomService
{
    /// <summary>Obtiene todas las aulas registradas en el sistema.</summary>
    Task<List<Classroom>> GetClassroomsAsync();

    /// <summary>Busca un aula específica mediante su identificador Guid.</summary>
    Task<Classroom?> GetClassroomByIdAsync(Guid id);

    /// <summary>Ejecuta las reglas de negocio y crea una nueva aula.</summary>
    Task CreateClassroomAsync(Classroom classroom);

    /// <summary>Actualiza la información de un aula existente.</summary>
    Task UpdateClassroomAsync(Classroom classroom);

    /// <summary>Elimina un aula por su identificador único.</summary>
    Task DeleteClassroomAsync(Guid id);

    /// <summary>Modifica el estado de activación del aula.</summary>
    Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);
}
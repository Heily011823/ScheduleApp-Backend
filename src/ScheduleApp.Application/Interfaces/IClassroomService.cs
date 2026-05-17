using ScheduleApp.Domain.Entities;
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces;

/*
 * Author: Salome Carmona
 * Feature: Classroom CRUD
 * Description: Service interface for classroom business logic
 *
 * Author: Mateo Quintero
 * Feature: #84 Validar código único de aula
 *          #85 Cambio de estado de aula
 */
public interface IClassroomService
{
    Task<List<Classroom>> GetClassroomsAsync();
    Task<Classroom?> GetClassroomByIdAsync(int id);
    Task CreateClassroomAsync(Classroom classroom);
    Task UpdateClassroomAsync(Classroom classroom);
    Task DeleteClassroomAsync(int id);

    /// <summary>
    /// Cambia el estado activo/inactivo de un aula.
    /// Rama: 85-implementar-cambio-de-estado-de-aula
    /// </summary>
    Task<Classroom?> ChangeStatusAsync(int id, bool isActive);
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;

// Ruta recomendada: src/ScheduleApp.Application/Interfaces/ITeacherService.cs
namespace ScheduleApp.Application.Interfaces
{
    /// <summary>
    /// Contrato del servicio de docentes.
    /// Define las operaciones de negocio disponibles en la capa de Aplicación.
    /// </summary>
    public interface ITeacherService
    {
        /// <summary>
        /// Obtiene la lista completa de docentes que están actualmente disponibles.
        /// </summary>
        Task<IEnumerable<Teacher>> GetAvailableTeachersAsync();

        /// <summary>
        /// Retorna una lista estructurada de docentes aplicando filtros relacionales directamente en el servidor.
        /// </summary>
        /// <param name="name">Filtro opcional por nombre o apellido.</param>
        /// <param name="academicProgram">Filtro opcional por programa académico/tipo de contrato.</param>
        /// <param name="status">Filtro de estado ("Activo"/"Inactivo" o "active"/"inactive").</param>
        Task<IEnumerable<TeacherResponseDto>> SearchAsync(
            string? name,
            string? academicProgram,
            string? status);

        /// <summary>
        /// Retorna el detalle completo de un docente específico por su ID.
        /// </summary>
        Task<TeacherResponseDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Crea un nuevo docente validando restricciones de duplicidad atómica.
        /// </summary>
        Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto);

        /// <summary>
        /// Actualiza la información de un docente existente validando unicidad de documento y correo.
        /// </summary>
        Task<TeacherResponseDto?> UpdateAsync(Guid id, UpdateTeacherDto dto);

        /// <summary>
        /// Realiza una eliminación lógica (Soft Delete) cambiando la bandera IsActive a false.
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Cambia explícitamente el estado activo/inactivo de un docente para habilitarlo o deshabilitarlo en las asignaciones.
        /// </summary>
        Task<TeacherResponseDto?> ChangeStatusAsync(Guid id, bool isActive);
    }
}
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
        /// Obtiene un docente por su documento de identidad.
        /// </summary>
        /// <param name="document">Número de documento de identidad.</param>
        Task<TeacherResponseDto?> GetByIdentityDocumentAsync(string document);

        /// <summary>
        /// Obtiene un docente por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del docente.</param>
        Task<TeacherResponseDto?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene todos los docentes activos (útiles para asignaciones y combos).
        /// </summary>
        Task<IEnumerable<TeacherResponseDto>> GetActiveTeachersAsync();

        /// <summary>
        /// Búsqueda avanzada de docentes por múltiples criterios.
        /// </summary>
        /// <param name="document">Filtro por número de documento (búsqueda parcial).</param>
        /// <param name="name">Filtro por nombre o apellido (búsqueda parcial).</param>
        /// <param name="email">Filtro por correo electrónico (búsqueda parcial).</param>
        /// <param name="academicProgram">Filtro por programa académico/especialidad.</param>
        /// <param name="status">Filtro de estado ("Activo"/"Inactivo" o "active"/"inactive").</param>
        Task<IEnumerable<TeacherResponseDto>> SearchAdvancedAsync(
            string? document,
            string? name,
            string? email,
            string? academicProgram,
            string? status);

        /// <summary>
        /// Búsqueda avanzada con paginación para mejorar el rendimiento en listados grandes.
        /// </summary>
        /// <param name="document">Filtro por número de documento (búsqueda parcial).</param>
        /// <param name="name">Filtro por nombre o apellido (búsqueda parcial).</param>
        /// <param name="email">Filtro por correo electrónico (búsqueda parcial).</param>
        /// <param name="academicProgram">Filtro por programa académico/especialidad.</param>
        /// <param name="contractType">Filtro por tipo de contrato (Tiempo Completo, Cátedra, etc).</param>
        /// <param name="status">Filtro de estado ("Activo"/"Inactivo" o "active"/"inactive").</param>
        /// <param name="page">Número de página (inicia en 1).</param>
        /// <param name="pageSize">Tamaño de página (máximo 50).</param>
        /// <returns>Tupla con la lista de docentes y el total de registros sin paginación.</returns>
        Task<(IEnumerable<TeacherResponseDto> Teachers, int TotalCount)> SearchAdvancedWithPaginationAsync(
            string? document,
            string? name,
            string? email,
            string? academicProgram,
            string? contractType,
            string? status,
            int page,
            int pageSize);

        /// <summary>
        /// Búsqueda rápida para autocompletado (sugerencias en tiempo real).
        /// Busca en documento, nombre completo y correo electrónico.
        /// </summary>
        /// <param name="term">Término de búsqueda (mínimo 2 caracteres).</param>
        /// <param name="limit">Máximo número de sugerencias a retornar.</param>
        /// <returns>Lista de objetos con ID, nombre completo, documento y correo para mostrar en sugerencias.</returns>
        Task<IEnumerable<object>> QuickSearchAsync(string term, int limit);

        /// <summary>
        /// Obtiene estadísticas agregadas de docentes para el dashboard.
        /// </summary>
        /// <returns>Objeto con totales, distribución por contrato y horas totales.</returns>
        Task<object> GetStatisticsAsync();

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



        /// <summary>
        /// Obtiene el horario/schedule de un docente específico
        /// </summary>
        /// <param name="id">ID del docente</param>
        /// <returns>Lista de disponibilidades del docente</returns>
        Task<IEnumerable<TeacherAvailabilityDto>> GetTeacherScheduleAsync(Guid id);


        Task<IEnumerable<TeacherResponseDto>> SearchAdvancedAsync(string? document, string? name, string? email, string? specialty, string? contractType, string? status);



    }
}
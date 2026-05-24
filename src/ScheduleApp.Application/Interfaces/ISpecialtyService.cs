using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{


    /// Autor:  Mateo Quintero 
    /// Version: 0.1
    /// rama: develop

    /// <summary>
    /// Contrato del servicio de especialidades.
    /// Define las operaciones de negocio disponibles para la gestión de especialidades.
    /// </summary>
    public interface ISpecialtyService
    {
        /// <summary>
        /// Obtiene todas las especialidades activas
        /// </summary>
        Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync();

        /// <summary>
        /// Obtiene una especialidad por su ID
        /// </summary>
        Task<SpecialtyDto?> GetSpecialtyByIdAsync(Guid id);

        /// <summary>
        /// Obtiene una especialidad por su nombre
        /// </summary>
        Task<SpecialtyDto?> GetSpecialtyByNameAsync(string name);

        /// <summary>
        /// Crea una nueva especialidad
        /// </summary>
        Task<SpecialtyDto> CreateSpecialtyAsync(CreateSpecialtyDto dto);

        /// <summary>
        /// Actualiza una especialidad existente
        /// </summary>
        Task<SpecialtyDto?> UpdateSpecialtyAsync(Guid id, UpdateSpecialtyDto dto);

        /// <summary>
        /// Elimina (desactiva) una especialidad
        /// </summary>
        Task<bool> DeleteSpecialtyAsync(Guid id);

        /// <summary>
        /// Valida que una lista de IDs de especialidades existan en la base de datos
        /// </summary>
        Task ValidateSpecialtyIdsExistAsync(IEnumerable<Guid> specialtyIds);

        /// <summary>
        /// Carga especialidades por defecto en la base de datos (solo usar una vez)
        /// </summary>
        Task<SeedResultDto> SeedSpecialtiesAsync(IEnumerable<object> defaultSpecialties);
    }
}
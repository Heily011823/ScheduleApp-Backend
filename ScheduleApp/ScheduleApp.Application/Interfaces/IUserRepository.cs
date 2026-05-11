using System;
// ScheduleApp.Application/Interfaces/IUserRepository.cs
// Contrato para el acceso a datos de usuarios.
// Permite que Application consulte y manipule usuarios sin depender de EF Core directamente.
/// Autor:  Mateo Quintero
/// Version: 0.1
namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Domain.Entities;
public interface IUserRepository
{
    /// <summary>
    /// Busca un usuario por su correo electrónico.
    /// </summary>
    /// <param name="email">Email a buscar. La implementación normaliza a minúsculas.</param>
    /// <returns>Usuario encontrado o null si no existe.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca un usuario por su identificador único.
    /// Necesario para los flujos de actualización y eliminación (soft delete).
    /// </summary>
    /// <param name="id">Identificador GUID del usuario.</param>
    /// <returns>Usuario encontrado o null si no existe.</returns>
    Task<User?> GetByIdAsync(Guid id);

    Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive);

    /// <summary>
    /// Persiste un nuevo usuario en la base de datos.
    /// </summary>
    /// <param name="user">Entidad de usuario lista para ser creada.</param>
    Task AddAsync(User user);

    /// <summary>
    /// Persiste los cambios de un usuario existente.
    /// También se usa para realizar soft delete (cambiando IsActive a false).
    /// </summary>
    /// <param name="user">Entidad de usuario con los cambios a aplicar.</param>
    Task UpdateAsync(User user);
}
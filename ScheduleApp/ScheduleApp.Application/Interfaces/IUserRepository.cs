using System;

/// Autor:  Mateo Quintero 
/// Version: 0.1

// ScheduleApp.Application/Interfaces/IUserRepository.cs
// Contrato para el acceso a datos de usuarios.
// Permite que Application consulte usuarios sin depender de EF Core directamente.
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

    Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive);
}

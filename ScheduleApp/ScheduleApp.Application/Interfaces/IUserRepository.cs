using System;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

/// <summary>
/// Contrato para acceso a datos de usuarios.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.2
public interface IUserRepository
{
    /// <summary>
    /// Busca un usuario por email.
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca un usuario por email o username.
    /// </summary>
    Task<User?> GetByEmailOrUsernameAsync(string login);

    /// <summary>
    /// Busca un usuario por Id.
    /// </summary>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca usuarios con filtros.
    /// </summary>
    Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive);

    /// <summary>
    /// Crear usuario.
    /// </summary>
    Task AddAsync(User user);

    /// <summary>
    /// Actualizar usuario.
    /// </summary>
    Task UpdateAsync(User user);
}
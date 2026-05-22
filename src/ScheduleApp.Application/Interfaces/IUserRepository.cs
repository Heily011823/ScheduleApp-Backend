using System;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<Guid?> GetRoleIdByNameAsync(string roleName);
    Task<User?> GetByEmailOrUsernameAsync(string login);
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive);

    Task<User?> GetByEmailIncludingDeletedAsync(string email);

    // Paginacion con filtros (HU-58).
    // Retorna los registros de la pagina pedida y el total de registros que matchean los filtros.
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        string? name,
        string? role,
        bool? isActive,
        int page,
        int pageSize);

    Task AddAsync(User user);
    Task UpdateAsync(User user);

    Task<User?> GetByIdentityDocumentAsync(string identityDocument);
    Task<User?> GetByUsernameAsync(string username);
}

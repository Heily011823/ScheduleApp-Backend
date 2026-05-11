using System;
// ScheduleApp.Infrastructure/Repositories/UserRepository.cs
// Implementación concreta de IUserRepository usando Entity Framework Core.
/// Autor:  Mateo Quintero
/// Version: 0.1
namespace ScheduleApp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
/// <summary>
/// Repositorio de usuarios. Encapsula las consultas a la tabla Users
/// usando EF Core. Implementa la interfaz definida en Application.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    /// <summary>
    /// Busca un usuario por email normalizando a minúsculas y eliminando espacios.
    /// Retorna null si no existe, permitiendo manejo limpio en AuthService.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());

    /// <summary>
    /// Busca un usuario por su Id (GUID). Usa FindAsync para aprovechar el cache
    /// de tracking de EF Core. Retorna null si no existe.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.FindAsync(id);

    public async Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive)
    {
        var query = _context.Users.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(u =>
                u.FullName.Contains(name));
        }
        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u =>
                u.Role == role);
        }
        if (isActive.HasValue)
        {
            query = query.Where(u =>
                u.IsActive == isActive.Value);
        }
        return await query.ToListAsync();
    }

    /// <summary>
    /// Agrega un nuevo usuario a la tabla Users y persiste los cambios.
    /// EF Core gestiona automáticamente el seguimiento de la nueva entidad.
    /// </summary>
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Marca la entidad como modificada y persiste los cambios en la base de datos.
    /// También se utiliza para realizar soft delete cambiando IsActive a false.
    /// </summary>
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
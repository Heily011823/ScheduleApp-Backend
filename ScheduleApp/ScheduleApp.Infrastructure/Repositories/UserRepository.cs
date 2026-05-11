using System;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio de usuarios.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.2
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca un usuario por email.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower().Trim();

        return await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email == normalizedEmail);
    }

    /// <summary>
    /// Busca un usuario por email o username.
    /// </summary>
    public async Task<User?> GetByEmailOrUsernameAsync(string login)
    {
        var normalizedLogin = login.ToLower().Trim();

        return await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email == normalizedLogin ||
                u.Username.ToLower() == normalizedLogin);
    }

    /// <summary>
    /// Busca usuario por Id.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    /// <summary>
    /// Buscar usuarios con filtros.
    /// </summary>
    public async Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u =>
                u.FullName.Contains(name) ||
                u.Username.Contains(name) ||
                u.Email.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(role))
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
    /// Crear usuario.
    /// </summary>
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualizar usuario.
    /// </summary>
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email == normalizedEmail &&
                !u.IsDeleted);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(string login)
    {
        var normalizedLogin = login.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                (u.Email == normalizedLogin ||
                 u.Username.ToLower() == normalizedLogin) &&
                !u.IsDeleted);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                !u.IsDeleted);
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

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
                u.Role.Name == role);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u =>
                u.IsActive == isActive.Value);
        }

        return await query.ToListAsync();
    }


    // Paginacion con filtros (HU-58).
    // Reutiliza la misma logica de filtros que SearchUsersAsync, aplica Skip/Take y devuelve el total.
    // Tambien filtra IsDeleted para no incluir usuarios eliminados logicamente.
    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        string? name,
        string? role,
        bool? isActive,
        int page,
        int pageSize)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u =>
                u.FullName.Contains(name) ||
                u.Username.Contains(name) ||
                u.Email.Contains(name));
        }
        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role.Name == role);
        }
        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}

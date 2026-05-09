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
}

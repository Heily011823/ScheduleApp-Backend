using System;

// ScheduleApp.Infrastructure/Repositories/UserRepository.cs
namespace ScheduleApp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
}

using System;

// ScheduleApp.Application/Interfaces/IUserRepository.cs
namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}

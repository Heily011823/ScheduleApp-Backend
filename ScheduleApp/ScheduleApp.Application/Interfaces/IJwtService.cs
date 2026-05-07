using System;

// ScheduleApp.Application/Interfaces/IJwtService.cs
namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Domain.Entities;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}
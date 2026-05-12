using System;

// ScheduleApp.Application/Interfaces/IRefreshTokenRepository.cs
namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllUserTokensAsync(Guid userId);
}

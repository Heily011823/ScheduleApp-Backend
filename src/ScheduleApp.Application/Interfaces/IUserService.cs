using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync();

    Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive);

    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);

    Task<UserResponseDto?> UpdateUserAsync(
        Guid id,
        UpdateUserDto dto);

    Task<bool> DeleteUserAsync(Guid id);
}
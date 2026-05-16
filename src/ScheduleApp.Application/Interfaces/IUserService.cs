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

    // Detalle de usuario por Id (HU-58)
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);

    // Paginacion con filtros (HU-58)
    Task<PagedResultDto<UserResponseDto>> GetPagedUsersAsync(
        string? name,
        string? role,
        bool? isActive,
        int page,
        int pageSize);

    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<UserResponseDto?> UpdateUserAsync(
        Guid id,
        UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid id);
}

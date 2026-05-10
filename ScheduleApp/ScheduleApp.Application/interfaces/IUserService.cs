using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive);
    }
}

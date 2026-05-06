using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
    }
}

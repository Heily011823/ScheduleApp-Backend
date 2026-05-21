using ScheduleApp.Application.interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Infrastructure.services
{
    public class UserService : IUserService
    {
        public Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = new List<User>
            {
                new User { Id = 1, Name = "Juan" },
                new User { Id = 2, Name = "Maria" }
            };

            return Task.FromResult<IEnumerable<User>>(users);
        }
    }
}
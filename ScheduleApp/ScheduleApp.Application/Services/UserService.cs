using ScheduleApp.Application.interfaces;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.SearchUsersAsync(
                null,
                null,
                null);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive)
        {
            return await _userRepository.SearchUsersAsync(
                name,
                role,
                isActive);
        }
    }
}
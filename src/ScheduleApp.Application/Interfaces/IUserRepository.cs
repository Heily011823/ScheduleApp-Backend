using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByEmailIncludingDeletedAsync(string email);

        Task<User?> GetByIdentityDocumentAsync(string identityDocument);

        Task<User?> GetByIdentityDocumentIncludingDeletedAsync(string identityDocument);

        Task<User?> GetByUsernameAsync(string username);

        Task<User?> GetByUsernameIncludingDeletedAsync(string username);

        Task<User?> GetByEmailOrUsernameAsync(string login);

        Task<User?> GetByIdAsync(Guid id);

        Task<Guid?> GetRoleIdByNameAsync(string roleName);
        
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive);

        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            string? name,
            string? role,
            bool? isActive,
            int page,
            int pageSize);

        Task AddAsync(User user);

        Task UpdateAsync(User user);
    }
}
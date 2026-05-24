using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        string normalizedEmail = email.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email == normalizedEmail &&
                !u.IsDeleted);
    }

    public async Task<User?> GetByEmailIncludingDeletedAsync(string email)
    {
        string normalizedEmail = email.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Email == normalizedEmail);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(string login)
    {
        string normalizedLogin = login.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                !u.IsDeleted &&
                (
                    u.Email == normalizedLogin ||
                    u.Username.ToLower() == normalizedLogin
                ));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(
        string? name,
        string? role,
        bool? isActive)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            string normalizedName = name.Trim();

            query = query.Where(u =>
                u.FullName.Contains(normalizedName) ||
                u.Username.Contains(normalizedName) ||
                u.Email.Contains(normalizedName));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            string normalizedRole = role.Trim();

            query = query.Where(u =>
                u.Role != null &&
                u.Role.Name == normalizedRole);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u =>
                u.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        string? name,
        string? role,
        bool? isActive,
        int page,
        int pageSize)
    {
        if (page <= 0)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var query = _context.Users
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            string normalizedName = name.Trim();

            query = query.Where(u =>
                u.FullName.Contains(normalizedName) ||
                u.Username.Contains(normalizedName) ||
                u.Email.Contains(normalizedName));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            string normalizedRole = role.Trim();

            query = query.Where(u =>
                u.Role != null &&
                u.Role.Name == normalizedRole);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u =>
                u.IsActive == isActive.Value);
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdentityDocumentAsync(string identityDocument)
    {
        string normalizedDocument = identityDocument.Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.IdentityDocument == normalizedDocument &&
                !u.IsDeleted);
    }

    public async Task<User?> GetByIdentityDocumentIncludingDeletedAsync(string identityDocument)
    {
        string normalizedDocument = identityDocument.Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.IdentityDocument == normalizedDocument);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        string normalizedUsername = username.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == normalizedUsername &&
                !u.IsDeleted);
    }

    public async Task<User?> GetByUsernameIncludingDeletedAsync(string username)
    {
        string normalizedUsername = username.ToLower().Trim();

        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == normalizedUsername);
    }

    public async Task<Guid?> GetRoleIdByNameAsync(string roleName)
    {
        string normalizedRoleName = roleName.Trim();

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == normalizedRoleName);

        return role?.Id;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }
}
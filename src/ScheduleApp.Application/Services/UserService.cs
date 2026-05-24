using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _userRepository.SearchUsersAsync(null, null, null);

            return users.Where(u => !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive)
        {
            var users = await _userRepository.SearchUsersAsync(name, role, isActive);

            return users.Where(u => !u.IsDeleted);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return null;
            }

            return MapToResponseDto(user);
        }

        public async Task<PagedResultDto<UserResponseDto>> GetPagedUsersAsync(
            string? name,
            string? role,
            bool? isActive,
            int page,
            int pageSize)
        {
            var (items, totalCount) = await _userRepository.GetPagedAsync(
                name, role, isActive, page, pageSize);

            var activeItems = items
                .Where(u => !u.IsDeleted)
                .ToList();

            var dtos = activeItems
                .Select(MapToResponseDto)
                .ToList();

            return new PagedResultDto<UserResponseDto>
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var normalizedEmail = dto.Email.ToLower().Trim();
            var trimmedDoc = dto.IdentityDocument.Trim();
            var trimmedUsername = dto.Username.Trim();

            var roleId = await _userRepository.GetRoleIdByNameAsync(dto.Role.Trim());

            if (roleId == null)
            {
                throw new ArgumentException($"El rol '{dto.Role}' no existe.");
            }

            var existingByEmail = await _userRepository.GetByEmailIncludingDeletedAsync(normalizedEmail);

            if (existingByEmail != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe un usuario registrado con el email '{normalizedEmail}'.");
            }

            var existingByDoc = await _userRepository.GetByIdentityDocumentAsync(trimmedDoc);

            if (existingByDoc != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe un usuario registrado con el documento '{trimmedDoc}'.");
            }

            var existingByUsername = await _userRepository.GetByUsernameAsync(trimmedUsername);

            if (existingByUsername != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe un usuario registrado con el username '{trimmedUsername}'.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName.Trim(),
                Email = normalizedEmail,
                Username = trimmedUsername,
                IdentityDocument = trimmedDoc,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                RoleId = roleId.Value,
                IsActive = dto.Status == "Activo",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return null;
            }

            var normalizedEmail = dto.Email.ToLower().Trim();
            var trimmedDoc = dto.IdentityDocument.Trim();
            var trimmedUsername = dto.Username.Trim();

            if (user.Email != normalizedEmail)
            {
                var existing = await _userRepository.GetByEmailIncludingDeletedAsync(normalizedEmail);

                if (existing != null && existing.Id != id)
                {
                    throw new InvalidOperationException(
                        $"Ya existe otro usuario con el email '{normalizedEmail}'.");
                }
            }

            if (user.IdentityDocument != trimmedDoc)
            {
                var existing = await _userRepository.GetByIdentityDocumentAsync(trimmedDoc);

                if (existing != null && existing.Id != id)
                {
                    throw new InvalidOperationException(
                        $"Ya existe otro usuario con el documento '{trimmedDoc}'.");
                }
            }

            if (user.Username != trimmedUsername)
            {
                var existing = await _userRepository.GetByUsernameAsync(trimmedUsername);

                if (existing != null && existing.Id != id)
                {
                    throw new InvalidOperationException(
                        $"Ya existe otro usuario con el username '{trimmedUsername}'.");
                }
            }

            var updatedRoleId = await _userRepository.GetRoleIdByNameAsync(dto.Role.Trim());

            if (updatedRoleId == null)
            {
                throw new ArgumentException($"El rol '{dto.Role}' no existe.");
            }

            user.FullName = dto.FullName.Trim();
            user.Email = normalizedEmail;
            user.Username = trimmedUsername;
            user.IdentityDocument = trimmedDoc;
            user.IsActive = dto.IsActive;
            user.RoleId = updatedRoleId.Value;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = _passwordHasher.Hash(dto.Password);
            }

            await _userRepository.UpdateAsync(user);

            return MapToResponseDto(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            if (user.IsDeleted)
            {
                return true;
            }

            user.IsDeleted = true;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                IdentityDocument = user.IdentityDocument,
                RoleName = user.Role?.Name ?? string.Empty,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
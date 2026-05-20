using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        private static readonly Guid AdministradorRoleId =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        private static readonly Guid CoordinadorRoleId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.SearchUsersAsync(null, null, null);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive)
        {
            return await _userRepository.SearchUsersAsync(name, role, isActive);
        }


        // Detalle de usuario por Id (HU-58). Devuelve null si no existe o esta eliminado.
        public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            return MapToResponseDto(user);
        }

        // Paginacion con filtros (HU-58).
        public async Task<PagedResultDto<UserResponseDto>> GetPagedUsersAsync(
            string? name,
            string? role,
            bool? isActive,
            int page,
            int pageSize)
        {
            var (items, totalCount) = await _userRepository.GetPagedAsync(
                name, role, isActive, page, pageSize);

            var dtos = items.Select(MapToResponseDto).ToList();

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

            var existing = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe un usuario registrado con el email '{normalizedEmail}'.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName.Trim(),
                Email = normalizedEmail,
                Username = dto.Username.Trim(),
                IdentityDocument = dto.IdentityDocument.Trim(),
                PasswordHash = _passwordHasher.Hash(dto.Password),
                RoleId = GetRoleId(dto.Role),
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

            if (user == null)
                return null;

            var normalizedEmail = dto.Email.ToLower().Trim();

            if (user.Email != normalizedEmail)
            {
                var existing = await _userRepository.GetByEmailAsync(normalizedEmail);

                if (existing != null && existing.Id != id)
                {
                    throw new InvalidOperationException(
                        $"Ya existe otro usuario registrado con el email '{normalizedEmail}'.");
                }
            }

            user.FullName = dto.FullName.Trim();
            user.Email = normalizedEmail;
            user.Username = dto.Username.Trim();
            user.IdentityDocument = dto.IdentityDocument.Trim();
            user.RoleId = GetRoleId(dto.Role);
            user.IsActive = dto.IsActive;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = _passwordHasher.Hash(dto.Password);
            }

            await _userRepository.UpdateAsync(user);

            return MapToResponseDto(user);
        }

        // Eliminacion logica del usuario (soft delete).
        // Cambia IsDeleted = true (permanente), no IsActive (que puede variar).
        // Siguiendo el patron pedido por Heili: IsActive es funcional, IsDeleted es definitivo.
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            if (user.IsDeleted)
                return true;

            user.IsDeleted = true;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        private static Guid GetRoleId(string role)
        {
            return role.Trim() == "Administrador"
                ? AdministradorRoleId
                : CoordinadorRoleId;
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
                CreatedAt = user.CreatedAt
            };
        }
    }
}

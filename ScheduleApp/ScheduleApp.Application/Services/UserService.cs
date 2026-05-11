using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.interfaces;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    /// <summary>
    /// Servicio de gestión de usuarios.
    /// Maneja validaciones, persistencia y seguridad.
    /// </summary>
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
            return await _userRepository.SearchUsersAsync(null, null, null);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive)
        {
            return await _userRepository.SearchUsersAsync(name, role, isActive);
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var normalizedEmail = dto.Email.ToLower().Trim();

            // Validar email único
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
                Role = dto.Role.Trim(),
                IsActive = dto.Status == "Activo",
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return MapToResponseDto(user);
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            var normalizedEmail = dto.Email.ToLower().Trim();

            // Validar email único si cambió
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
            user.Role = dto.Role.Trim();
            user.IsActive = dto.IsActive;

            await _userRepository.UpdateAsync(user);

            return MapToResponseDto(user);
        }

        /// <summary>
        /// Borrado lógico de usuario.
        /// </summary>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            if (!user.IsActive)
                return true;

            user.IsActive = false;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        /// <summary>
        /// Convierte User → UserResponseDto
        /// </summary>
        private static UserResponseDto MapToResponseDto(User user) => new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Username = user.Username,
            IdentityDocument = user.IdentityDocument,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
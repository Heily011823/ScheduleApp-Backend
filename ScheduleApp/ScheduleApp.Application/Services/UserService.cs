using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.interfaces;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.Services
{
    /// <summary>
    /// Servicio de gestión de usuarios. Orquesta validaciones de negocio,
    /// hasheo de contraseñas y persistencia a través del repositorio.
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
        /// Crea un nuevo usuario. Normaliza el email a minúsculas, valida que
        /// no exista otro con el mismo email y hashea la contraseña con BCrypt.
        /// </summary>
        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var normalizedEmail = dto.Email.ToLower().Trim();

            // Validación de unicidad del email
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
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Role = dto.Role.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return MapToResponseDto(user);
        }

        /// <summary>
        /// Actualiza un usuario existente. Si el email cambió, valida unicidad.
        /// Retorna null si no se encontró el usuario.
        /// </summary>
        public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var normalizedEmail = dto.Email.ToLower().Trim();

            // Si el email cambió, validar que no esté en uso por otro usuario
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
        /// Realiza borrado lógico marcando IsActive como false.
        /// Idempotente: si ya estaba inactivo, retorna true sin persistir cambios.
        /// </summary>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            if (!user.IsActive) return true; // Ya estaba inactivo, idempotente

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        /// <summary>
        /// Mapea una entidad User a su DTO de respuesta omitiendo el PasswordHash.
        /// Centraliza el mapeo para mantener DRY y evitar fugas de datos sensibles.
        /// </summary>
        private static UserResponseDto MapToResponseDto(User user) => new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
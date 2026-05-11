using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.interfaces
{
    /// <summary>
    /// Contrato del servicio de gestión de usuarios.
    /// Define las operaciones de consulta y mutación expuestas a la capa API.
    /// </summary>
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<IEnumerable<User>> SearchUsersAsync(
            string? name,
            string? role,
            bool? isActive);

        /// <summary>
        /// Crea un nuevo usuario en el sistema. Hashea la contraseña, normaliza
        /// el email y valida unicidad antes de persistir.
        /// </summary>
        /// <param name="dto">Datos del usuario a crear.</param>
        /// <returns>DTO de respuesta del usuario creado (sin PasswordHash).</returns>
        /// <exception cref="InvalidOperationException">Si el email ya está en uso.</exception>
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);

        /// <summary>
        /// Actualiza los datos de un usuario existente. Valida unicidad del email
        /// solo si este cambió. No modifica la contraseña.
        /// </summary>
        /// <param name="id">Identificador del usuario a actualizar.</param>
        /// <param name="dto">Nuevos datos del usuario.</param>
        /// <returns>DTO del usuario actualizado, o null si no se encontró.</returns>
        /// <exception cref="InvalidOperationException">Si el nuevo email ya está en uso por otro usuario.</exception>
        Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);

        /// <summary>
        /// Realiza borrado lógico (soft delete) marcando IsActive en false.
        /// Es idempotente: si el usuario ya estaba inactivo, retorna true sin cambios.
        /// </summary>
        /// <param name="id">Identificador del usuario a eliminar.</param>
        /// <returns>True si fue eliminado o ya estaba inactivo; false si no existe.</returns>
        Task<bool> DeleteUserAsync(Guid id);
    }
}
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.interfaces;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.API.Controllers
{
    /// <summary>
    /// Controlador de gestión de usuarios. Expone los endpoints REST para
    /// consultar, crear, editar y eliminar usuarios del sistema.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lista todos los usuarios. Soporta filtros opcionales por nombre, rol y estado activo.
        /// </summary>
        /// <returns>200 OK con la lista de usuarios; 500 si ocurre un error interno.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] string? name,
            [FromQuery] string? role,
            [FromQuery] bool? isActive)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(
                    name,
                    role,
                    isActive);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="dto">Datos del usuario a crear (validados automáticamente por DataAnnotations).</param>
        /// <returns>201 Created con el usuario creado; 400 si la validación falla; 409 si el email ya está en uso; 500 si ocurre un error.</returns>
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser(
            [FromBody] CreateUserDto dto)
        {
            try
            {
                var created = await _userService.CreateUserAsync(dto);
                return Created($"/api/users/{created.Id}", created);
            }
            catch (InvalidOperationException ex)
            {
                // Email duplicado u otra regla de negocio violada
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="id">Identificador GUID del usuario.</param>
        /// <param name="dto">Nuevos datos del usuario.</param>
        /// <returns>200 OK con el usuario actualizado; 404 si no existe; 409 si el email choca con otro usuario; 500 si ocurre un error.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(
            Guid id,
            [FromBody] UpdateUserDto dto)
        {
            try
            {
                var updated = await _userService.UpdateUserAsync(id, dto);
                if (updated == null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un usuario con el Id '{id}'."
                    });
                }
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un usuario del sistema mediante borrado lógico (soft delete:
        /// marca IsActive en false en lugar de borrar la fila para preservar
        /// integridad referencial e historial).
        /// </summary>
        /// <param name="id">Identificador GUID del usuario a eliminar.</param>
        /// <returns>204 NoContent si fue eliminado o ya estaba inactivo; 404 si no existe; 500 si ocurre un error.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un usuario con el Id '{id}'."
                    });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
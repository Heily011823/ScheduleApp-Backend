using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.API.Controllers
{
    /// <summary>
    /// Controlador de gestión de usuarios.
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
        /// Lista usuarios.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers(
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

                var response = users.Select(user => new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Username = user.Username,
                    IdentityDocument = user.IdentityDocument,
                    RoleName = user.Role.Name,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crear usuario.
        /// </summary>
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
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualizar usuario.
        /// </summary>
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
        /// Eliminar usuario.
        /// </summary>
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
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.API.Controllers
{
    // Controlador de gestion de usuarios.
    // Expone los endpoints REST para CRUD y consultas (con paginacion).
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/Users (HU-58) - Listado paginado de usuarios con filtros opcionales.
        // Conserva los filtros previos (name, role, isActive) y agrega page/pageSize.
        // Retorna 200 con PagedResultDto que incluye items, totalCount, page, pageSize, totalPages.
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<UserResponseDto>>> GetUsers(
            [FromQuery] string? name,
            [FromQuery] string? role,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Sanitizamos valores fuera de rango
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var result = await _userService.GetPagedUsersAsync(
                    name,
                    role,
                    isActive,
                    page,
                    pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET /api/Users/{id} (HU-58) - Detalle de un usuario por su Id.
        // Retorna 200 con los datos completos, 404 si no existe.
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontro un usuario con el Id '{id}'."
                    });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST /api/Users - Crea un nuevo usuario.
        // Retorna 201 con el usuario creado, 409 si email duplicado.
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

        // PUT /api/Users/{id} - Actualiza los datos de un usuario existente.
        // Retorna 200 con el usuario actualizado, 404 si no existe, 409 si email duplicado.
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
                        message = $"No se encontro un usuario con el Id '{id}'."
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

        // DELETE /api/Users/{id} - Soft delete (marca IsActive=false).
        // Retorna 204 sin body, 404 si no existe.
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
                        message = $"No se encontro un usuario con el Id '{id}'."
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

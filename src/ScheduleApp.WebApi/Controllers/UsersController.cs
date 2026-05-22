using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.API.Controllers
{
    /// <summary>
    /// Controlador de gestión de usuarios.
    /// Expone los endpoints REST para CRUD y consultas (con paginación).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Constructor del controlador de usuarios.
        /// </summary>

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un listado paginado de usuarios con filtros opcionales.
        /// </summary>
        /// <remarks>
        /// Permite filtrar usuarios por nombre, rol y estado activo.
        /// Los resultados se pagan automáticamente para mejorar el rendimiento.

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDto<UserResponseDto>>> GetUsers(
            [FromQuery] string? name,
            [FromQuery] string? role,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios paginados - Página: {Page}, Tamaño: {PageSize}, Filtros - Nombre: {Name}, Rol: {Role}, Activo: {IsActive}",
                    page, pageSize, name ?? "ninguno", role ?? "ninguno", isActive);

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

                _logger.LogInformation("Se encontraron {TotalCount} usuarios", result.TotalCount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios paginados");
                return StatusCode(500, new { message = "Error interno del servidor al obtener los usuarios" });
            }
        }

        /// <summary>
        /// Obtiene los detalles completos de un usuario específico por su ID.
        /// </summary>
        /// <remarks>
        /// Retorna toda la información del usuario incluyendo datos personales, rol y estado.

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
        {
            try
            {
                _logger.LogInformation("Buscando usuario con ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado con ID: {UserId}", id);
                    return NotFound(new
                    {
                        message = $"No se encontró un usuario con el Id '{id}'."
                    });
                }

                _logger.LogInformation("Usuario encontrado: {UserName} con ID: {UserId}", user.FullName, id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor al obtener el usuario" });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// </summary>
        /// <remarks>
        /// Valida que el email, username y documento de identidad sean únicos.
        /// La contraseña debe cumplir con los requisitos de seguridad mínimos.

        [HttpPost]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponseDto>> CreateUser(
            [FromBody] CreateUserDto dto)
        {
            try
            {
                // Validar el modelo automáticamente
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creando nuevo usuario con email: {Email}", dto.Email);

                var created = await _userService.CreateUserAsync(dto);

                _logger.LogInformation("Usuario creado exitosamente con ID: {UserId} y email: {Email}",
                    created.Id, created.Email);

                return Created($"/api/users/{created.Id}", created);
            }
            // En UsersController.CreateUser, agrega este catch antes del genérico
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("IX_Users_Email") == true)
            {
                return Conflict(new { message = "Ya existe un usuario con ese email." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("CHK_User_Email") == true)
            {
                return BadRequest(new { message = "El email debe pertenecer al dominio @autonoma.edu.co" });
            }
            catch (InvalidOperationException ex)
            {
                // Captura excepciones de validación de negocio (email duplicado, documento duplicado, etc.)
                _logger.LogWarning("Conflicto al crear usuario: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Captura errores de validación de argumentos
                _logger.LogWarning("Datos inválidos al crear usuario: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al crear usuario con email: {Email}", dto.Email);
                return StatusCode(500, new { message = "Error interno del servidor al crear el usuario" });
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos personales del usuario.
        /// Valida que el email no esté siendo usado por otro usuario.
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(
            Guid id,
            [FromBody] UpdateUserDto dto)
        {
            try
            {
                // Validar el modelo automáticamente
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Actualizando usuario con ID: {UserId}", id);

                var updated = await _userService.UpdateUserAsync(id, dto);

                if (updated == null)
                {
                    _logger.LogWarning("Usuario no encontrado para actualizar con ID: {UserId}", id);
                    return NotFound(new
                    {
                        message = $"No se encontró un usuario con el Id '{id}'."
                    });
                }

                _logger.LogInformation("Usuario actualizado exitosamente con ID: {UserId}", id);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflicto al actualizar usuario {UserId}: {Message}", id, ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Datos inválidos al actualizar usuario {UserId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("CHK_User_Email") == true)
            {
                return BadRequest(new { message = "El email debe pertenecer al dominio @autonoma.edu.co" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor al actualizar el usuario" });
            }
        }

        /// <summary>
        /// Elimina (soft delete) un usuario del sistema.
        /// </summary>
        /// <remarks>
        /// Realiza un borrado lógico del usuario, marcando IsActive = false.
        /// El usuario no se elimina físicamente de la base de datos.

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                _logger.LogInformation("Eliminando (soft delete) usuario con ID: {UserId}", id);

                var deleted = await _userService.DeleteUserAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Usuario no encontrado para eliminar con ID: {UserId}", id);
                    return NotFound(new
                    {
                        message = $"No se encontró un usuario con el Id '{id}'."
                    });
                }

                _logger.LogInformation("Usuario eliminado exitosamente con ID: {UserId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al eliminar usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor al eliminar el usuario" });
            }
        }
    }
}
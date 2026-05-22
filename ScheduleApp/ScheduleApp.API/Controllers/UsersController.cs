// ScheduleApp.API/Controllers/UsersController.cs

/// Autor: Juan José Morales Aristizabal
/// Version: 0.1

// Controlador encargado de gestionar los usuarios del sistema.
// Permite consultar y filtrar usuarios según diferentes
// criterios como nombre, rol y estado.

using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // ── Servicio de usuarios ────────────────────────────────────────────
        // Servicio encargado de la lógica de negocio relacionada
        // con la gestión y búsqueda de usuarios.
        private readonly IUserService _userService;

        // ── Constructor ─────────────────────────────────────────────────────
        // Inyección de dependencias del servicio de usuarios.
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ── Obtener usuarios ────────────────────────────────────────────────
        // Endpoint HTTP GET encargado de obtener la lista de usuarios.
        // Permite filtrar por:
        //   - Nombre del usuario.
        //   - Rol asignado.
        //   - Estado de activación.
        // Respuestas:
        //   - 200 OK → usuarios obtenidos correctamente.
        //   - 500 Internal Server Error → error interno del servidor.
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
    }
}
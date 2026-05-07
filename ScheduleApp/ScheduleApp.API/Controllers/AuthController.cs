using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private static AuthService service = new AuthService();

        [HttpPost("validar")]
        public IActionResult ValidarAcceso(
            [FromBody] Usuario usuario,
            [FromQuery] string modulo)
        {
            var tienePermiso = service.TienePermiso(usuario, modulo);

            if (!tienePermiso)
            {
                return BadRequest("No tienes permisos para acceder a este módulo");
            }

            return Ok("Acceso permitido");
        }
    }
}
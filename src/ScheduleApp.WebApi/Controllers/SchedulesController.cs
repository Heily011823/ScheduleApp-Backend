// Autor: Jacobo
// Version: 0.1

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleGenerationService _generationService;

        public SchedulesController(IScheduleGenerationService generationService)
        {
            _generationService = generationService;
        }

        // Genera automáticamente un horario para el programa, semestre y jornada solicitados.
        // 200 OK: si pudo asignar al menos una materia.
        // 422 Unprocessable Entity: si no pudo asignar ninguna materia o el input es inválido.
        // 400 Bad Request: si el cuerpo de la petición está malformado.
        [HttpPost("generate")]
        public async Task<IActionResult> Generate(
            [FromBody] GenerateScheduleRequestDto request)
        {
            try
            {
                if (request is null)
                {
                    return BadRequest(new
                    {
                        message = "El cuerpo de la petición no puede ser nulo."
                    });
                }

                var result = await _generationService.GenerateAsync(request);

                if (!result.Success)
                {
                    return UnprocessableEntity(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al generar el horario.",
                    detail = ex.Message
                });
            }
        }
    }
}
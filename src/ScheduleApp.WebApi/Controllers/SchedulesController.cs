// Autor: Jacobo
// Version: 0.1
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleGenerationService _generationService;

        public SchedulesController(IScheduleGenerationService generationService)
        {
            _generationService = generationService;
        }

        // Genera automaticamente un horario para el programa, semestre y jornada solicitados.
        // - 200 OK: si pudo asignar al menos una materia (puede traer warnings).
        // - 422 Unprocessable Entity: si no pudo asignar ninguna materia o el input es invalido.
        // - 400 Bad Request: si el cuerpo de la peticion esta malformado.
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateScheduleRequestDto request)
        {
            if (request is null)
            {
                return BadRequest(new { message = "El cuerpo de la peticion no puede ser nulo." });
            }

            var result = await _generationService.GenerateAsync(request);

            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }

            return Ok(result);
        }
    }
}

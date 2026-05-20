// Autor: Jacobo
// Version: 0.2

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
        private readonly IScheduleService _scheduleService;

        public SchedulesController(
            IScheduleGenerationService generationService,
            IScheduleService scheduleService)
        {
            _generationService = generationService;
            _scheduleService = scheduleService;
        }

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

        [HttpPost("save")]
        public async Task<IActionResult> Save(
            [FromBody] SaveScheduleRequestDto request)
        {
            try
            {
                if (request is null || request.Schedules.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Debe enviar al menos un horario para guardar."
                    });
                }

                await _scheduleService.SaveAsync(request);

                return Ok(new
                {
                    message = "Horario guardado correctamente."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al guardar el horario.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByFilters(
            [FromQuery] string academicProgram,
            [FromQuery] string shift,
            [FromQuery] int semester)
        {
            try
            {
                var result = await _scheduleService.GetByFiltersAsync(
                    academicProgram,
                    shift,
                    semester
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al consultar los horarios.",
                    detail = ex.Message
                });
            }
        }
    }
}
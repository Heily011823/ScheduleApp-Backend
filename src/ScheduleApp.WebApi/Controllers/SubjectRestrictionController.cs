using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.WebApi.Controllers;

[ApiController]
[Route("api/subject-restrictions")]
public class SubjectRestrictionController
    : ControllerBase
{
    private readonly ISubjectRestrictionService
        _service;

    public SubjectRestrictionController(
        ISubjectRestrictionService service)
    {
        _service = service;
    }

    // ==========================================
    // CREAR RESTRICCIÓN
    // ==========================================
    [HttpPost]
    public async Task<IActionResult> CreateRestriction(
        CreateSubjectRestrictionDto dto)
    {
        try
        {
            await _service.CreateRestrictionAsync(dto);

            return Ok(new
            {
                message =
                    "Restricción creada correctamente."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // ==========================================
    // OBTENER RESTRICCIONES POR MATERIA
    // ==========================================
    [HttpGet("{subjectId:guid}")]
    public async Task<IActionResult>
        GetRestrictionsBySubjectId(
            Guid subjectId)
    {
        try
        {
            var restrictions =
                await _service
                    .GetRestrictionsBySubjectIdAsync(
                        subjectId);

            return Ok(restrictions);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // ==========================================
    // ELIMINAR RESTRICCIONES
    // ==========================================
    [HttpDelete("{subjectId:guid}")]
    public async Task<IActionResult>
        DeleteRestrictions(
            Guid subjectId)
    {
        try
        {
            await _service
                .DeleteRestrictionsBySubjectIdAsync(
                    subjectId);

            return Ok(new
            {
                message =
                    "Restricciones eliminadas correctamente."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
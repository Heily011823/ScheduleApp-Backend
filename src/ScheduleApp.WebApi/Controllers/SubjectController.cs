using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.WebApi.Controllers;

[ApiController]
[Route("api/subjects")]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    // MODIFICADO: Ahora el endpoint recibe 'page' y 'pageSize' desde el Query String.
    // Se configuran valores por defecto (Página 1, 10 registros por página) si no se envían.
    [HttpGet("search")]
    public async Task<IActionResult> SearchSubjects(
        [FromQuery] string? search,
        [FromQuery] int? semester,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            // Se invoca al servicio pasándole las variables matemáticas de paginación
            var pagedResult = await _subjectService.SearchSubjectsAsync(
                search,
                semester,
                isActive,
                page,
                pageSize);

            // Retorna un PagedResultDto<Subject> con la metadata y el estatus 200 OK
            return Ok(pagedResult);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        try
        {
            await _subjectService.DeleteSubjectAsync(id);
            return Ok("Materia eliminada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubject(CreateSubjectDto dto)
    {
        try
        {
            await _subjectService.CreateSubjectAsync(dto);
            return Ok("Materia creada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSubject(Guid id, UpdateSubjectDto dto)
    {
        try
        {
            await _subjectService.UpdateSubjectAsync(id, dto);
            return Ok("Materia actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportSubjectsToExcel()
    {
        var file = await _subjectService.ExportSubjectsToExcelAsync();

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "materias.xlsx"
        );
    }
}
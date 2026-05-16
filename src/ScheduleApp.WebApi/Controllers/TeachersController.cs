// src/ScheduleApp.WebApi/Controllers/TeachersController.cs
namespace ScheduleApp.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

/// <summary>
/// Controlador para gestión de docentes del sistema académico.
/// Permite al coordinador crear, consultar, editar y eliminar docentes.
/// Ruta base: /api/teachers
/// </summary>
/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 96-Crud-docentes
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService) =>
        _teacherService = teacherService;

    /// <summary>
    /// Retorna la lista de docentes con filtros opcionales.
    /// Criterio: dado que se consulta el listado, cuando existan docentes,
    /// entonces el sistema los devuelve correctamente.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? specialty,
        [FromQuery] bool? isActive)
    {
        try
        {
            var teachers = await _teacherService.SearchAsync(
                name, specialty, isActive);
            return Ok(teachers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al consultar docentes.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Retorna un docente específico por su ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher is null)
                return NotFound(new
                {
                    message = $"No se encontró un docente con el ID '{id}'."
                });
            return Ok(teacher);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al consultar el docente.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Crea un nuevo docente en el sistema.
    /// Valida que email y documento sean únicos.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto)
    {
        try
        {
            var teacher = await _teacherService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = teacher.Id }, teacher);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al crear el docente.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Actualiza los datos de un docente existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTeacherDto dto)
    {
        try
        {
            var teacher = await _teacherService.UpdateAsync(id, dto);
            if (teacher is null)
                return NotFound(new
                {
                    message = $"No se encontró un docente con el ID '{id}'."
                });
            return Ok(teacher);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al actualizar el docente.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Desactiva un docente (eliminación lógica).
    /// El docente no se borra físicamente para preservar historial.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _teacherService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new
                {
                    message = $"No se encontró un docente con el ID '{id}'."
                });
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al eliminar el docente.",
                detail = ex.Message
            });
        }
    }
}
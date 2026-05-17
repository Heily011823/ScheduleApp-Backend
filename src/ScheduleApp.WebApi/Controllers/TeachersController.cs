using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

// Ruta recomendada: src/ScheduleApp.WebApi/Controllers/TeachersController.cs
namespace ScheduleApp.WebApi.Controllers;

/// <summary>
/// Controlador para gestión de docentes del sistema académico.
/// Permite crear, consultar, actualizar y desactivar docentes.
/// Ruta base: /api/teachers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    /*
 * Author: Salome Carmona
 * Feature: Available Teachers
 * Description: Endpoint to retrieve active teachers
 */

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableTeachers()
    {
        var teachers = await _teacherService.GetAvailableTeachersAsync();

        return Ok(teachers);
    }
    /// <summary>
    /// Retorna la lista de docentes con filtros opcionales.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? academicProgram,
        [FromQuery] string? status) // Cambiado a string para alinearse con ITeacherService
    {
        try
        {
            // Ahora los parámetros coinciden exactamente con la firma de ITeacherService
            var teachers = await _teacherService.SearchAsync(
                name,
                academicProgram,
                status);

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
            {
                return NotFound(new
                {
                    message = $"No se encontró un docente con el ID '{id}'."
                });
            }

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
    /// Crea un nuevo docente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto)
    {
        try
        {
            var teacher = await _teacherService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = teacher.Id },
                teacher);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
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
    /// Actualiza la información de un docente existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherDto dto)
    {
        try
        {
            var teacher = await _teacherService.UpdateAsync(id, dto);

            if (teacher is null)
            {
                return NotFound(new
                {
                    message = $"No se encontró un docente con el ID '{id}'."
                });
            }

            return Ok(teacher);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
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
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _teacherService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"No se encontró un docente con el ID '{id}'."
                });
            }

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



    /// <summary>
    /// Cambia el estado activo/inactivo de un docente.
    /// Dado que está activo y se desactiva → no aparece en asignaciones.
    /// Dado que está inactivo y se activa → vuelve a estar disponible.
    /// </summary>
    /// Autor: Mateo Quintero
    /// Version: 0.1
    /// Rama: 99-implementar-cambio-de-estado-de-docente
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusDto dto)
    {
        try
        {
            var teacher = await _teacherService.ChangeStatusAsync(id, dto.IsActive);
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
                message = "Error al cambiar el estado del docente.",
                detail = ex.Message
            });
        }
    }
}
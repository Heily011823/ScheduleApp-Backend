// src/ScheduleApp.WebApi/Controllers/ClassroomsController.cs
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.WebApi.Controllers;

/*
 * Author: Salome Carmona
 * Feature: Classroom CRUD
 * Description: API endpoints for classroom management
 *
 * Author: Mateo Quintero
 * Feature: #84 Validar código único de aula
 *          #85 Cambio de estado de aula
 */
[ApiController]
[Route("api/[controller]")]
public class ClassroomsController : ControllerBase
{
    private readonly IClassroomService _classroomService;

    public ClassroomsController(IClassroomService classroomService)
    {
        _classroomService = classroomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var classrooms = await _classroomService.GetClassroomsAsync();
            return Ok(classrooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al consultar aulas.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Crea un aula nueva.
    /// Criterio #84: retorna 409 si el código ya existe.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Classroom classroom)
    {
        try
        {
            await _classroomService.CreateClassroomAsync(classroom);
            return Ok(classroom);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al crear el aula.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Actualiza un aula existente.
    /// Criterio #84: retorna 409 si el código ya existe en otra aula.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Classroom classroom)
    {
        try
        {
            classroom.Id = id;
            await _classroomService.UpdateClassroomAsync(classroom);
            return Ok(classroom);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al actualizar el aula.",
                detail = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _classroomService.DeleteClassroomAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al eliminar el aula.",
                detail = ex.Message
            });
        }
    }
}
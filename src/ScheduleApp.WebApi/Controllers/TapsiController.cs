// ScheduleApp.WebApi/Controllers/TapsiController.cs
namespace ScheduleApp.WebApi.Controllers;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Services;

/// <summary>
/// Controlador para gestión de reglas especiales TAPSI.
/// Permite crear, consultar, editar y eliminar reglas,
/// y validar asignaciones contra dichas reglas.
/// Ruta base: /api/tapsi
/// </summary>

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TapsiController : ControllerBase
{
    private readonly TapsiService _tapsiService;

    public TapsiController(TapsiService tapsiService) =>
        _tapsiService = tapsiService;

    /// <summary>
    /// Retorna todas las reglas TAPSI registradas.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rules = await _tapsiService.GetAllRulesAsync();
        return Ok(rules);
    }

    /// <summary>
    /// Retorna una regla TAPSI por su ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var rule = await _tapsiService.GetRuleByIdAsync(id);
            return Ok(rule);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva regla TAPSI.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TapsiRuleDto dto)
    {
        try
        {
            var rule = await _tapsiService.CreateRuleAsync(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = rule.Id }, rule);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear la regla.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una regla TAPSI existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TapsiRuleDto dto)
    {
        try
        {
            var rule = await _tapsiService.UpdateRuleAsync(id, dto);
            return Ok(rule);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar la regla.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una regla TAPSI por su ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _tapsiService.DeleteRuleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar la regla.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Valida una asignación de horario contra las reglas TAPSI activas.
    /// Si la materia no es TAPSI, retorna válido sin advertencias.
    /// Si alguna regla no se cumple, retorna advertencias al coordinador.
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] TapsiValidationRequestDto request)
    {
        try
        {
            var result = await _tapsiService.ValidateAssignmentAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al validar reglas TAPSI.", detail = ex.Message });
        }
    }
}
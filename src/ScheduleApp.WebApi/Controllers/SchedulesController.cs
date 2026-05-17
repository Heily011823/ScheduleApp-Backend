// src/ScheduleApp.WebApi/Controllers/SchedulesController.cs
namespace ScheduleApp.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

/// <summary>
/// Controlador para gestión de horarios con validación de créditos.
/// Ruta base: /api/schedules
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
/// Rama: 143-validar-creditos-scheduleApp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly CreditValidationService _creditValidationService;
    private readonly IScheduleRepository _scheduleRepository;

    public SchedulesController(
        CreditValidationService creditValidationService,
        IScheduleRepository scheduleRepository)
    {
        _creditValidationService = creditValidationService;
        _scheduleRepository = scheduleRepository;
    }

    /// <summary>
    /// Crea una nueva asignación de horario validando el límite de créditos.
    /// Criterio: si los créditos superan el límite → 400 Bad Request.
    /// Criterio: si hay espacio → guarda y retorna 201 Created.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Schedule schedule)
    {
        try
        {
            // Validar créditos antes de guardar
            await _creditValidationService.ValidateAsync(
                schedule.SubjectId,
                schedule.AcademicProgram,
                schedule.Semester);

            var created = await _scheduleRepository.CreateAsync(schedule);
            return CreatedAtAction(nameof(Create),
                new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            // Criterio: retorna 400 con mensaje específico
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
}

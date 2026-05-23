// Autor: Jacobo
// Version: 0.2

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.WebApi.Controllers;

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
    private readonly IScheduleGenerationService _generationService;
    private readonly IScheduleService _scheduleService;
    private readonly IPdfExportService _pdfService;
    private readonly IExcelExportService _excelService;

    // Constructor único que inicializa TODOS los servicios
    public SchedulesController(
        CreditValidationService creditValidationService,
        IScheduleRepository scheduleRepository,
        IScheduleGenerationService generationService,
        IScheduleService scheduleService,
        IPdfExportService pdfService, IExcelExportService excelService)
    {
        _creditValidationService = creditValidationService;
        _scheduleRepository = scheduleRepository;
        _generationService = generationService;
        _scheduleService = scheduleService;
        _pdfService = pdfService;
        _excelService = excelService;
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

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportPdf(

        [FromQuery] string academicProgram,
        [FromQuery] string shift,
        [FromQuery] int semester)
        {
        try
        {
            var schedules =
            await _scheduleService
            .GetByFiltersAsync(
                academicProgram,
                shift,
                semester
            );

            if (!schedules.Any())
            {
                return NotFound(
                new
                {
                    message =
                    "No existen horarios."
                });
            }

            var pdf =
            _pdfService
            .GenerateSchedulePdf(
                schedules
            );

            return File(
                pdf,
                "application/pdf",
                $"Horario_{semester}.pdf"
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
            500,
            new
            {
                message =
                "Error exportando PDF",
                detail = ex.Message
            });
        }
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] string academicProgram,
        [FromQuery] string shift,
        [FromQuery] int semester)

        {
        try
        {
            var schedules =
            await _scheduleService
            .GetByFiltersAsync(
                academicProgram,
                shift,
                semester
            );

            if (!schedules.Any())
            {
                return NotFound(
                new
                {
                    message =
                    "No existen horarios."
                });
            }

            var excel =
            _excelService
            .GenerateScheduleExcel(
                schedules
            );

            return File(
                excel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Horario_{semester}.xlsx"
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
            500,
            new
            {
                message =
                "Error exportando Excel",
                detail =
                ex.Message
            });
        }
    }
}
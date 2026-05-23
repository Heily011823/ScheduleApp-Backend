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
/// Controlador encargado de la gestión de horarios.
/// Permite:
/// - Crear horarios
/// - Generar horarios automáticos
/// - Guardar horarios
/// - Consultar horarios mediante filtros
/// - Exportar horarios en PDF
/// - Exportar horarios en Excel
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.2
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

    /// <summary>
    /// Constructor principal del controlador.
    /// Inicializa todos los servicios requeridos.
    /// </summary>
    public SchedulesController(
        CreditValidationService creditValidationService,
        IScheduleRepository scheduleRepository,
        IScheduleGenerationService generationService,
        IScheduleService scheduleService,
        IPdfExportService pdfService,
        IExcelExportService excelService)
    {
        _creditValidationService = creditValidationService;
        _scheduleRepository = scheduleRepository;
        _generationService = generationService;
        _scheduleService = scheduleService;
        _pdfService = pdfService;
        _excelService = excelService;
    }

    /// <summary>
    /// Crea un horario nuevo validando límite de créditos.
    /// </summary>
    /// <param name="schedule">Horario a registrar.</param>
    /// <returns>Horario creado.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Schedule schedule)
    {
        try
        {
            await _creditValidationService.ValidateAsync(
                schedule.SubjectId,
                schedule.AcademicProgram,
                schedule.Semester
            );

            var created =
                await _scheduleRepository.CreateAsync(
                    schedule
                );

            return CreatedAtAction(
                nameof(Create),
                new { id = created.Id },
                created
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new { message = ex.Message }
            );
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new { message = ex.Message }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    message = "Error al guardar horario.",
                    detail = ex.Message
                }
            );
        }
    }

    /// <summary>
    /// Genera automáticamente un horario.
    /// </summary>
    /// <param name="request">
    /// Programa, semestre y jornada.
    /// </param>
    /// <returns>Horario generado.</returns>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromBody]
        GenerateScheduleRequestDto request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(
                new
                {
                    message =
                    "Petición inválida."
                });
            }

            var result =
                await _generationService
                .GenerateAsync(request);

            if (!result.Success)
            {
                return UnprocessableEntity(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(
            500,
            new
            {
                message =
                "Error generando horario.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Guarda horarios generados.
    /// </summary>
    [HttpPost("save")]
    public async Task<IActionResult> Save(
        [FromBody]
        SaveScheduleRequestDto request)
    {
        try
        {
            if (request == null ||
                request.Schedules.Count == 0)
            {
                return BadRequest(
                new
                {
                    message =
                    "Debe enviar horarios."
                });
            }

            await _scheduleService
                .SaveAsync(request);

            return Ok(
            new
            {
                message =
                "Horario guardado correctamente."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(
            500,
            new
            {
                message =
                "Error guardando horario.",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Consulta horarios usando filtros.
    /// Permite filtrar por:
    /// programa, jornada,
    /// semestre y estado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByFilters(

        [FromQuery]
        string academicProgram = "",

        [FromQuery]
        string shift = "",

        [FromQuery]
        int semester = 0,

        [FromQuery]
        string status = "")
    {
        try
        {
            var result =
            await _scheduleService
            .GetByFiltersAsync(
                academicProgram,
                shift,
                semester,
                status
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(
            500,
            new
            {
                message =
                "Error consultando horarios.",
                detail =
                ex.Message
            });
        }
    }

    /// <summary>
    /// Exporta horarios filtrados a PDF.
    /// </summary>
    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportPdf(

        [FromQuery]
        string academicProgram,

        [FromQuery]
        string shift,

        [FromQuery]
        int semester)
    {
        try
        {
            var schedules =
            await _scheduleService
            .GetByFiltersAsync(
                academicProgram,
                shift,
                semester,
                ""
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
                "Error exportando PDF.",
                detail =
                ex.Message
            });
        }
    }

    /// <summary>
    /// Exporta horarios filtrados a Excel.
    /// </summary>
    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel(

        [FromQuery]
        string academicProgram,

        [FromQuery]
        string shift,

        [FromQuery]
        int semester)
    {
        try
        {
            var schedules =
            await _scheduleService
            .GetByFiltersAsync(
                academicProgram,
                shift,
                semester,
                ""
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
                "Error exportando Excel.",
                detail =
                ex.Message
            });
        }
    }
}
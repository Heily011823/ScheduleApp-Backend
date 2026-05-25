// src/ScheduleApp.WebApi/Controllers/ClassroomsController.cs
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ScheduleApp.WebApi.Controllers;

/*
 * Authors: Salome Carmona, Mateo Quintero
 * Description: API endpoints for classroom management and availability using Guid identifiers
 * Features: Classroom CRUD, #84 Validar código único, #85 Cambio de estado de aula
 */
[ApiController]
[Route("api/[controller]")]
public class ClassroomsController : ControllerBase
{
    private readonly IClassroomService _classroomService;
    private readonly IClassroomAvailabilityService _availabilityService;
    private readonly AppDbContext _context;

    public ClassroomsController(
        IClassroomService classroomService,
        IClassroomAvailabilityService availabilityService, AppDbContext context)
    {
        _classroomService = classroomService;
        _availabilityService = availabilityService;
        _context = context;
    }


    // src/ScheduleApp.WebApi/Controllers/ClassroomsController.cs
    // Reemplazar el método GetAll() (líneas 34-46) con:
    /// <summary>
    /// Obtiene la lista de aulas registradas, opcionalmente filtrada por nombre, código o edificio.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        string? name = null,
        string? code = null,
        string? building = null)
    {
        try
        {
            var classrooms = await _classroomService.GetClassroomsAsync(name, code, building);
            return Ok(classrooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al consultar aulas.", detail = ex.Message });
        }
    }


    /// <summary>
    /// Obtiene un aula específica por su identificador único.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var classroom = await _classroomService.GetClassroomByIdAsync(id);
            if (classroom is null)
            {
                return NotFound(new { message = $"No se encontró un aula con el ID '{id}'." });
            }
            return Ok(classroom);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al consultar el aula.", detail = ex.Message });
        }
    }



    /// <summary>
    /// Crea un aula nueva.
    /// Criterio #84: Retorna 409 Conflict si el código de aula ya existe.
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
            return StatusCode(500, new { message = "Error al crear el aula.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza los datos de un aula existente pasándole su Guid por la URL.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Classroom classroom)
    {
        try
        {
            classroom.Id = id; // Asignación consistente de Guid
            await _classroomService.UpdateClassroomAsync(classroom);
            return Ok(classroom);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar el aula.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Elimina físicamente un aula por su Guid.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _classroomService.DeleteClassroomAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar el aula.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Cambia el estado activo/inactivo de un aula.
    /// Feature #85: Si se desactiva, no aparecerá en las asignaciones disponibles.
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusDto dto)
    {
        try
        {
            var classroom = await _classroomService.ChangeStatusAsync(id, dto.IsActive);
            if (classroom is null)
            {
                return NotFound(new { message = $"No se encontró un aula con el ID '{id}'." });
            }
            return Ok(classroom);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al cambiar el estado del aula.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Verifica la disponibilidad horaria y registra la asignación de un aula.
    /// URL de acceso: POST /api/classrooms/assignment
    /// </summary>
    [HttpPost("assignment")]
    public IActionResult CheckAvailability([FromBody] ClassroomAssignment assignment)
    {
        if (assignment == null)
            return BadRequest(new { success = false, message = "La asignación es obligatoria." });

        if (assignment.ClassroomId == Guid.Empty)
            return BadRequest(new { success = false, message = "Debe seleccionar un aula válida." });

        if (assignment.Date == default)
            return BadRequest(new { success = false, message = "La fecha es obligatoria." });

        if (assignment.StartTime == default || assignment.EndTime == default)
            return BadRequest(new { success = false, message = "El horario es obligatorio." });

        if (assignment.StartTime >= assignment.EndTime)
            return BadRequest(new { success = false, message = "La hora de inicio debe ser menor que la hora de fin." });

        var result = _availabilityService.SaveAssignment(assignment);

        if (result.Contains("ocupada"))
            return BadRequest(new { success = false, message = result });

        return Ok(new
        {
            success = true,
            message = result,
            dayOfWeek = assignment.Date.DayOfWeek.ToString()
        });
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var classrooms = await _context.Classrooms
            .AsQueryable()
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Aulas");

        worksheet.Cell(1, 1).Value = "Código";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "Edificio";
        worksheet.Cell(1, 4).Value = "Piso";
        worksheet.Cell(1, 5).Value = "Capacidad";
        worksheet.Cell(1, 6).Value = "Tipo";
        worksheet.Cell(1, 7).Value = "Estado";

        var headerRange = worksheet.Range(1, 1, 1, 7);

        headerRange.Style.Font.Bold = true;

        headerRange.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#1B2A41");

        headerRange.Style.Font.FontColor =
            XLColor.White;

        int row = 2;

        foreach (var c in classrooms)
        {
            worksheet.Cell(row, 1).Value = c.Code;
            worksheet.Cell(row, 2).Value = c.Name;
            worksheet.Cell(row, 3).Value = c.Building;
            worksheet.Cell(row, 4).Value = c.Floor;
            worksheet.Cell(row, 5).Value = c.Capacity;
            worksheet.Cell(row, 6).Value = c.Type;
            worksheet.Cell(row, 7).Value =
                c.IsActive ? "Activa" : "Inactiva";

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var content = stream.ToArray();

        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "aulas.xlsx");
    }
}
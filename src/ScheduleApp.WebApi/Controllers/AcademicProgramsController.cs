using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using System.IO;

namespace ScheduleApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[AllowAnonymous]
public class AcademicProgramsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AcademicProgramsController(AppDbContext context)
    {
        _context = context;
    }

    // Lista los programas no eliminados. Filtro opcional por nombre.
    // Autor: Mateo Quintero (HU-136), modificado por Jacobo (HU-127) para filtrar IsDeleted.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name)
    {
        try
        {
            // Filtro principal: nunca retornar programas eliminados logicamente.
            var query = _context.AcademicPrograms
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.ToLower().Trim();
                query = query.Where(p => p.Name.ToLower().Contains(normalizedName));
            }

            var programs = await query
                .OrderBy(p => p.Code)
                .ToListAsync();

            return Ok(programs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al consultar programas.",
                detail = ex.Message
            });
        }
    }

    // Crea un nuevo programa academico e inicializa automaticamente sus semestres.
    // Autor: Jacobo - HU-142
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAcademicProgramDto dto)
    {
        const int DefaultMaxCredits = 16;

        if (dto is null)
            return BadRequest(new { message = "El cuerpo de la peticion no puede ser nulo." });

        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { message = "El codigo es requerido." });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "El nombre es requerido." });

        if (string.IsNullOrWhiteSpace(dto.Shift))
            return BadRequest(new { message = "La jornada es requerida." });

        if (dto.TotalSemesters <= 0 || dto.TotalSemesters > 20)
            return BadRequest(new { message = "El numero de semestres debe estar entre 1 y 20." });

        var codeNormalized = dto.Code.Trim();
        var exists = await _context.AcademicPrograms
            .AnyAsync(p => p.Code == codeNormalized && !p.IsDeleted);
        if (exists)
            return Conflict(new { message = $"Ya existe un programa con codigo '{codeNormalized}'." });

        var program = new AcademicProgram
        {
            Id = Guid.NewGuid(),
            Code = codeNormalized,
            Name = dto.Name.Trim(),
            Shift = dto.Shift.Trim(),
            TotalSemesters = dto.TotalSemesters,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.AcademicPrograms.Add(program);

        for (int i = 1; i <= dto.TotalSemesters; i++)
        {
            _context.ProgramSemesters.Add(new ProgramSemester
            {
                Id = Guid.NewGuid(),
                AcademicProgramId = program.Id,
                SemesterNumber = i,
                MaxCredits = DefaultMaxCredits,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = program.Id }, new
        {
            program.Id,
            program.Code,
            program.Name,
            program.Shift,
            program.TotalSemesters,
            program.IsActive,
            program.IsDeleted,
            program.CreatedAt,
            SemestersInitialized = dto.TotalSemesters,
            DefaultMaxCreditsPerSemester = DefaultMaxCredits
        });
    }

    // Edita un programa academico existente.
    // No permite cambiar TotalSemesters (porque romperia los ProgramSemesters ya creados).
    // No toca IsDeleted (para eso esta el endpoint DELETE).
    // Autor: Jacobo - HU-127
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAcademicProgramDto dto)
    {
        if (dto is null)
            return BadRequest(new { message = "El cuerpo de la peticion no puede ser nulo." });

        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { message = "El codigo es requerido." });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "El nombre es requerido." });

        if (string.IsNullOrWhiteSpace(dto.Shift))
            return BadRequest(new { message = "La jornada es requerida." });

        // Solo se editan programas no eliminados logicamente.
        var program = await _context.AcademicPrograms
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (program is null)
            return NotFound(new { message = $"No se encontro el programa con Id '{id}'." });

        // Verificar que el nuevo codigo no este en uso por OTRO programa no eliminado.
        var codeNormalized = dto.Code.Trim();
        var duplicateCode = await _context.AcademicPrograms
            .AnyAsync(p => p.Code == codeNormalized && p.Id != id && !p.IsDeleted);
        if (duplicateCode)
            return Conflict(new { message = $"Ya existe otro programa con codigo '{codeNormalized}'." });

        program.Code = codeNormalized;
        program.Name = dto.Name.Trim();
        program.Shift = dto.Shift.Trim();
        program.IsActive = dto.IsActive;
        program.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            program.Id,
            program.Code,
            program.Name,
            program.Shift,
            program.TotalSemesters,
            program.IsActive,
            program.IsDeleted,
            program.UpdatedAt
        });
    }

    // Eliminacion logica del programa academico (soft delete).
    // Cambia IsDeleted = true y mantiene el registro en BD.
    // Validacion: no se puede eliminar un programa que tenga horarios asociados.
    // Autor: Jacobo - HU-127
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var program = await _context.AcademicPrograms
            .FirstOrDefaultAsync(p => p.Id == id);

        if (program is null)
            return NotFound(new { message = $"No se encontro el programa con Id '{id}'." });

        if (program.IsDeleted)
            return Ok(new
            {
                message = $"El programa '{program.Name}' ya estaba eliminado.",
                program.Id,
                program.IsDeleted
            });

        // Validar que no haya horarios amarrados al programa.
        // Schedule.AcademicProgram es un string (nombre del programa), no FK.
        var hasSchedules = await _context.Schedules
            .AnyAsync(s => s.AcademicProgram == program.Name);

        if (hasSchedules)
            return Conflict(new
            {
                message = $"No se puede eliminar el programa '{program.Name}' porque tiene horarios asociados."
            });

        program.IsDeleted = true;
        program.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = $"El programa '{program.Name}' fue eliminado correctamente.",
            program.Id,
            program.Name,
            program.IsDeleted,
            program.UpdatedAt
        });
    }

    // Exporta el listado de programas no eliminados.
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var programs = await _context.AcademicPrograms
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Code)
            .ToListAsync();

        var html = new StringBuilder();
        html.AppendLine("<html><body>");
        html.AppendLine("<h1>Reporte de Programas Academicos</h1>");
        html.AppendLine("<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;width:100%'>");
        html.AppendLine("<thead><tr>");
        html.AppendLine("<th>Codigo</th><th>Nombre</th><th>Jornada</th><th>No Semestres</th><th>Estado</th>");
        html.AppendLine("</tr></thead><tbody>");

        foreach (var p in programs)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{p.Code}</td>");
            html.AppendLine($"<td>{p.Name}</td>");
            html.AppendLine($"<td>{p.Shift}</td>");
            html.AppendLine($"<td>{p.TotalSemesters}</td>");
            html.AppendLine($"<td>{(p.IsActive ? "Activa" : "Inactiva")}</td>");
            html.AppendLine("</tr>");
        }

        html.AppendLine("</tbody></table></body></html>");

        var htmlBytes = Encoding.UTF8.GetBytes(html.ToString());

        return File(htmlBytes, "text/html", "reporte_programas_academicos.html");
    }

    // Exporta el listado de programas no eliminados en Excel.
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var programs = await _context.AcademicPrograms
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Code)
            .ToListAsync();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Programas");

            worksheet.Cell(1, 1).Value = "Codigo";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Jornada";
            worksheet.Cell(1, 4).Value = "Numero de Semestres";
            worksheet.Cell(1, 5).Value = "Estado";

            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;

            int row = 2;
            foreach (var p in programs)
            {
                worksheet.Cell(row, 1).Value = p.Code;
                worksheet.Cell(row, 2).Value = p.Name;
                worksheet.Cell(row, 3).Value = p.Shift;
                worksheet.Cell(row, 4).Value = p.TotalSemesters;
                worksheet.Cell(row, 5).Value = p.IsActive ? "Activa" : "Inactiva";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "programas_academicos.xlsx"
                );
            }
        }
    }
}

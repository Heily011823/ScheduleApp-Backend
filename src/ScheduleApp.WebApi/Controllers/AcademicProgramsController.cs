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

    /// <summary>
    /// Retorna programas academicos con filtro opcional por nombre.
    /// Criterio: si el parametro viene vacio retorna todos sin filtros.
    /// Criterio: la busqueda ignora mayusculas, minusculas y es parcial (LIKE).
    /// </summary>
    /// Autor: Mateo Quintero
    /// Version: 0.1
    /// Rama: 136-busqueda-nombre-programa
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name)
    {
        try
        {
            var query = _context.AcademicPrograms.AsQueryable();

            // Criterio: si name viene vacio retorna todo sin filtros
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criterio: ignora mayusculas y minusculas (LIKE %texto%)
                var normalizedName = name.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(normalizedName));
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
    // Por cada semestre creado se inserta una fila en ProgramSemesters con
    // un valor por defecto de creditos (DefaultMaxCredits).
    // Autor: Jacobo - HU-142
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAcademicProgramDto dto)
    {
        // Creditos por defecto que se asignan a cada semestre nuevo.
        // Si despues quieren personalizarlos, se editan via el endpoint PUT de program-semesters.
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

        // Verificar codigo unico
        var codeNormalized = dto.Code.Trim();
        var exists = await _context.AcademicPrograms.AnyAsync(p => p.Code == codeNormalized);
        if (exists)
            return Conflict(new { message = $"Ya existe un programa con codigo '{codeNormalized}'." });

        // Crear programa academico
        var program = new AcademicProgram
        {
            Id = Guid.NewGuid(),
            Code = codeNormalized,
            Name = dto.Name.Trim(),
            Shift = dto.Shift.Trim(),
            TotalSemesters = dto.TotalSemesters,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.AcademicPrograms.Add(program);

        // Inicializacion automatica de semestres (CDA HU-142)
        // Se crean en cascada N filas en ProgramSemesters, una por cada semestre del 1 a N.
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

        // Un solo SaveChanges, queda transaccional: si algo falla, no se guarda nada.
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = program.Id }, new
        {
            program.Id,
            program.Code,
            program.Name,
            program.Shift,
            program.TotalSemesters,
            program.IsActive,
            program.CreatedAt,
            SemestersInitialized = dto.TotalSemesters,
            DefaultMaxCreditsPerSemester = DefaultMaxCredits
        });
    }

    /// <summary>
    /// Exporta el listado de programas academicos en formato PDF (actualmente HTML).
    /// </summary>
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var programs = await _context.AcademicPrograms
            .OrderBy(p => p.Code)
            .ToListAsync();

        // Generamos HTML para convertir a PDF con una libreria simple
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

    /// <summary>
    /// Exporta el listado de programas academicos en formato Excel.
    /// </summary>
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var programs = await _context.AcademicPrograms
            .OrderBy(p => p.Code)
            .ToListAsync();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Programas");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Codigo";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Jornada";
            worksheet.Cell(1, 4).Value = "Numero de Semestres";
            worksheet.Cell(1, 5).Value = "Estado";

            // Estilo encabezados
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

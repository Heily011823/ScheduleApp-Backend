using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    /// Retorna programas académicos con filtro opcional por nombre.
    /// Criterio: si el parámetro viene vacío → retorna todos sin filtros.
    /// Criterio: la búsqueda ignora mayúsculas, minúsculas y es parcial (LIKE).
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

            // Criterio: si name viene vacío retorna todo sin filtros
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Criterio: ignora mayúsculas y minúsculas (LIKE %texto%)
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

    /// <summary>
    /// Exporta el listado de programas académicos en formato PDF.
    /// </summary>
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var programs = await _context.AcademicPrograms
            .OrderBy(p => p.Code)
            .ToListAsync();

        // Generamos HTML para convertir a PDF con una librería simple
        var html = new StringBuilder();
        html.AppendLine("<html><body>");
        html.AppendLine("<h1>Reporte de Programas Académicos</h1>");
        html.AppendLine("<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;width:100%'>");
        html.AppendLine("<thead><tr>");
        html.AppendLine("<th>Código</th><th>Nombre</th><th>Jornada</th><th>Nº Semestres</th><th>Estado</th>");
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
    /// Exporta el listado de programas académicos en formato Excel.
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
            worksheet.Cell(1, 1).Value = "Código";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "Jornada";
            worksheet.Cell(1, 4).Value = "Número de Semestres";
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
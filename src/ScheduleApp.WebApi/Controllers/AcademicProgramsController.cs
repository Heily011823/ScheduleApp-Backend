using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Infrastructure.Data;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var programs = await _context.AcademicPrograms
            .OrderBy(p => p.Code)
            .ToListAsync();
        return Ok(programs);
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
}
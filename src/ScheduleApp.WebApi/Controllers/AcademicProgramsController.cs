using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
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

    private static bool _questPdfInitialized = false;
    private static readonly object _questPdfLock = new();

    public AcademicProgramsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name)
    {
        try
        {
            var query = _context.AcademicPrograms
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.ToLower().Trim();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(normalizedName) ||
                    p.Code.ToLower().Contains(normalizedName));
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAcademicProgramDto dto)
    {
        if (dto is null)
        {
            return BadRequest(new
            {
                message = "El cuerpo de la peticion no puede ser nulo."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            return BadRequest(new
            {
                message = "El codigo es requerido."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new
            {
                message = "El nombre es requerido."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Shift))
        {
            return BadRequest(new
            {
                message = "La jornada es requerida."
            });
        }

        if (dto.TotalSemesters <= 0 || dto.TotalSemesters > 20)
        {
            return BadRequest(new
            {
                message = "El numero de semestres debe estar entre 1 y 20."
            });
        }

        var codeNormalized = dto.Code.Trim();

        var exists = await _context.AcademicPrograms
            .AnyAsync(p => p.Code == codeNormalized && !p.IsDeleted);

        if (exists)
        {
            return Conflict(new
            {
                message = $"Ya existe un programa con codigo '{codeNormalized}'."
            });
        }

        var program = new AcademicProgram
        {
            Id = Guid.NewGuid(),
            Code = codeNormalized,
            Name = dto.Name.Trim(),
            Shift = CleanComboBoxValue(dto.Shift),
            TotalSemesters = dto.TotalSemesters,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AcademicPrograms.Add(program);

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
            program.UpdatedAt
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAcademicProgramDto dto)
    {
        if (dto is null)
        {
            return BadRequest(new
            {
                message = "El cuerpo de la peticion no puede ser nulo."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            return BadRequest(new
            {
                message = "El codigo es requerido."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new
            {
                message = "El nombre es requerido."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Shift))
        {
            return BadRequest(new
            {
                message = "La jornada es requerida."
            });
        }

        if (dto.TotalSemesters <= 0 || dto.TotalSemesters > 20)
        {
            return BadRequest(new
            {
                message = "El numero de semestres debe estar entre 1 y 20."
            });
        }

        var program = await _context.AcademicPrograms
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (program is null)
        {
            return NotFound(new
            {
                message = $"No se encontro el programa con Id '{id}'."
            });
        }

        var codeNormalized = dto.Code.Trim();

        var duplicateCode = await _context.AcademicPrograms
            .AnyAsync(p => p.Code == codeNormalized && p.Id != id && !p.IsDeleted);

        if (duplicateCode)
        {
            return Conflict(new
            {
                message = $"Ya existe otro programa con codigo '{codeNormalized}'."
            });
        }

        program.Code = codeNormalized;
        program.Name = dto.Name.Trim();
        program.Shift = CleanComboBoxValue(dto.Shift);
        program.TotalSemesters = dto.TotalSemesters;
        program.IsActive = dto.IsActive;
        program.UpdatedAt = DateTime.UtcNow;

        var semestersToRemove = await _context.ProgramSemesters
            .Where(s => s.AcademicProgramId == id &&
                        s.SemesterNumber > dto.TotalSemesters)
            .ToListAsync();

        if (semestersToRemove.Count > 0)
        {
            _context.ProgramSemesters.RemoveRange(semestersToRemove);
        }

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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var program = await _context.AcademicPrograms
            .FirstOrDefaultAsync(p => p.Id == id);

        if (program is null)
        {
            return NotFound(new
            {
                message = $"No se encontro el programa con Id '{id}'."
            });
        }

        if (program.IsDeleted)
        {
            return Ok(new
            {
                message = $"El programa '{program.Name}' ya estaba eliminado.",
                program.Id,
                program.IsDeleted
            });
        }

        var hasSchedules = await _context.Schedules
            .AnyAsync(s => s.AcademicProgram == program.Name);

        if (hasSchedules)
        {
            return Conflict(new
            {
                message = $"No se puede eliminar el programa '{program.Name}' porque tiene horarios asociados."
            });
        }

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

    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        EnsureQuestPdfLicense();

        var programs = await _context.AcademicPrograms
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Code)
            .ToListAsync();

        var generatedAt = DateTime.Now;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(headerCol =>
                {
                    headerCol.Item().Text("Reporte de Programas Academicos")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    headerCol.Item().PaddingTop(4)
                        .Text($"Generado el: {generatedAt:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .AlignCenter()
                        .FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    if (programs.Count == 0)
                    {
                        col.Item().PaddingTop(40).AlignCenter()
                            .Text("No hay programas registrados.")
                            .FontSize(12)
                            .Italic()
                            .FontColor(Colors.Grey.Darken2);

                        return;
                    }

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(4);
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Codigo").FontColor(Colors.White).Bold();

                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Nombre").FontColor(Colors.White).Bold();

                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Jornada").FontColor(Colors.White).Bold();

                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Semestres").FontColor(Colors.White).Bold();

                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Estado").FontColor(Colors.White).Bold();
                        });

                        int rowIndex = 0;

                        foreach (var p in programs)
                        {
                            var bgColor = rowIndex % 2 == 0
                                ? Colors.White
                                : Colors.Grey.Lighten4;

                            table.Cell().Background(bgColor).Padding(5).Text(p.Code);
                            table.Cell().Background(bgColor).Padding(5).Text(p.Name);
                            table.Cell().Background(bgColor).Padding(5).Text(CleanComboBoxValue(p.Shift));
                            table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(p.TotalSemesters.ToString());
                            table.Cell().Background(bgColor).Padding(5).Text(p.IsActive ? "Activa" : "Inactiva");

                            rowIndex++;
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1));
                    text.Span("Pagina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        });

        var pdfBytes = document.GeneratePdf();

        return File(
            pdfBytes,
            "application/pdf",
            $"programas_academicos_{generatedAt:yyyyMMdd_HHmm}.pdf"
        );
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var programs = await _context.AcademicPrograms
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Code)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Programas");

        worksheet.Cell(1, 1).Value = "Codigo";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "Jornada";
        worksheet.Cell(1, 4).Value = "Numero de Semestres";
        worksheet.Cell(1, 5).Value = "Estado";

        var headerRange = worksheet.Range(1, 1, 1, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1B2A41");
        headerRange.Style.Font.FontColor = XLColor.White;

        int row = 2;

        foreach (var p in programs)
        {
            worksheet.Cell(row, 1).Value = p.Code;
            worksheet.Cell(row, 2).Value = p.Name;
            worksheet.Cell(row, 3).Value = CleanComboBoxValue(p.Shift);
            worksheet.Cell(row, 4).Value = p.TotalSemesters;
            worksheet.Cell(row, 5).Value = p.IsActive ? "Activa" : "Inactiva";

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var content = stream.ToArray();

        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "programas_academicos.xlsx"
        );
    }

    private static void EnsureQuestPdfLicense()
    {
        if (_questPdfInitialized)
            return;

        lock (_questPdfLock)
        {
            if (_questPdfInitialized)
                return;

            QuestPDF.Settings.License = LicenseType.Community;
            _questPdfInitialized = true;
        }
    }

    private static string CleanComboBoxValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string cleanValue = value.Trim();

        if (cleanValue.Contains("ComboBoxItem:"))
        {
            cleanValue = cleanValue.Split(':')[^1].Trim();
        }

        return cleanValue;
    }
}
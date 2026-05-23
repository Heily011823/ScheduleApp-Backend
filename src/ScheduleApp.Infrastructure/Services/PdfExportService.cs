using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.Infrastructure.Services;

public class PdfExportService : IPdfExportService
{
    public byte[] GenerateSchedulePdf(
        List<GeneratedScheduleEntryDto> schedules)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);

                page.Header()
                .Text("HORARIO ACADÉMICO")
                .FontSize(18)
                .Bold();

                page.Content()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);

                        columns.RelativeColumn(2);

                        columns.RelativeColumn();

                        columns.RelativeColumn();

                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Materia");
                        header.Cell().Text("Docente");
                        header.Cell().Text("Aula");
                        header.Cell().Text("Día");
                        header.Cell().Text("Hora");
                    });

                    foreach (var item in schedules)
                    {
                        table.Cell()
                        .Text(item.SubjectName);

                        table.Cell()
                        .Text(item.TeacherFullName);

                        table.Cell()
                        .Text(item.ClassroomName);

                        table.Cell()
                        .Text(GetDay(item.Day));

                        table.Cell()
                        .Text(
                        $"{item.StartTime:hh\\:mm} - {item.EndTime:hh\\:mm}"
                        );
                    }
                });

                page.Footer()
                .AlignCenter()
                .Text("ScheduleApp");
            });

        }).GeneratePdf();
    }

    private string GetDay(int day)
    {
        return day switch
        {
            1 => "Lunes",
            2 => "Martes",
            3 => "Miércoles",
            4 => "Jueves",
            5 => "Viernes",
            _ => ""
        };
    }
}
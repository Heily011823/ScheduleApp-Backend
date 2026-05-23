using ClosedXML.Excel;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] GenerateScheduleExcel(
        List<GeneratedScheduleEntryDto> schedules)
    {
        using var workbook = new XLWorkbook();

        var sheet =
            workbook.Worksheets.Add("Horarios");

        sheet.Cell(1, 1).Value = "Materia";
        sheet.Cell(1, 2).Value = "Docente";
        sheet.Cell(1, 3).Value = "Aula";
        sheet.Cell(1, 4).Value = "Día";
        sheet.Cell(1, 5).Value = "Hora Inicio";
        sheet.Cell(1, 6).Value = "Hora Fin";
        sheet.Cell(1, 7).Value = "Programa";
        sheet.Cell(1, 8).Value = "Jornada";

        var row = 2;

        foreach (var item in schedules)
        {
            sheet.Cell(row, 1)
                .Value = item.SubjectName;

            sheet.Cell(row, 2)
                .Value = item.TeacherFullName;

            sheet.Cell(row, 3)
                .Value = item.ClassroomName;

            sheet.Cell(row, 4)
                .Value = GetDay(item.Day);

            sheet.Cell(row, 5)
                .Value =
                item.StartTime.ToString();

            sheet.Cell(row, 6)
                .Value =
                item.EndTime.ToString();

            sheet.Cell(row, 7)
                .Value =
                item.AcademicProgram;

            sheet.Cell(row, 8)
                .Value =
                item.Shift;

            row++;
        }

        sheet.Columns()
            .AdjustToContents();

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
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
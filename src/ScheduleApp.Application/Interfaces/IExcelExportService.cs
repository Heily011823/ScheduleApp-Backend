using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces;

public interface IExcelExportService
{
    byte[] GenerateScheduleExcel(
        List<GeneratedScheduleEntryDto> schedules
    );
}
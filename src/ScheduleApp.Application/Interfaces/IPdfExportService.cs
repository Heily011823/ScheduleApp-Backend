using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces;

public interface IPdfExportService
{
    byte[] GenerateSchedulePdf(
        List<GeneratedScheduleEntryDto> schedules
    );
}
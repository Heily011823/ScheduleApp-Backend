// Autor: Jacobo
// Version: 0.1

using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    public interface IScheduleService
    {
        Task SaveAsync(
            SaveScheduleRequestDto request
        );

        Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(
            string academicProgram,
            string shift,
            int semester,
            string status
        );
    }
}
// Autor: Jacobo
// Version: 0.2

using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    public interface IScheduleRepository
    {
        Task<bool> AcademicProgramExistsAsync(Guid academicProgramId);

        Task<List<GeneratedScheduleEntryDto>> GetSubjectsForGenerationAsync(
            Guid academicProgramId,
            int semesterNumber,
            string shift
        );

        Task SaveAsync(
            List<GeneratedScheduleEntryDto> schedules
        );

        Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(
            string academicProgram,
            string shift,
            int semester
        );
    }
}
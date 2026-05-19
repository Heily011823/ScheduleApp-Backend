// Autor: Jacobo
// Version: 0.1

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
    }
}
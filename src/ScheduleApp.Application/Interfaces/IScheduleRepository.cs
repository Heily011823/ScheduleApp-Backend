using ScheduleApp.Application.DTOs;

public interface IScheduleRepository
{
    Task<bool> AcademicProgramExistsAsync(Guid academicProgramId);

    Task<List<GeneratedScheduleEntryDto>> GetSubjectsForGenerationAsync(
        Guid academicProgramId,
        int semesterNumber,
        string shift
    );

    Task SaveAsync(List<GeneratedScheduleEntryDto> schedules);

    Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(
        string academicProgram,
        string shift,
        int semester
    );
}
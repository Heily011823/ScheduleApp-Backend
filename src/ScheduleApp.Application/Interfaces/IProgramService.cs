using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    public interface IProgramService
    {
        Task<List<ProgramResponseDto>> GetAllAsync();
        Task<ProgramResponseDto?> GetByIdAsync(Guid id);
        Task<ProgramResponseDto> CreateAsync(CreateProgramDto dto);
        Task<ProgramResponseDto?> UpdateAsync(Guid id, UpdateProgramDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}

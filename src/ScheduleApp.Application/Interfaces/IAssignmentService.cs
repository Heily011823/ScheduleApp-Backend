using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<AssignmentResponseDto> CreateAssignmentAsync(CreateAssignmentDto dto);

        Task<List<AssignmentResponseDto>> GetAssignmentsAsync();

        Task<AssignmentResponseDto?> GetAssignmentByIdAsync(int id);

        Task<AssignmentResponseDto?> UpdateAssignmentAsync(
            int id,
            UpdateAssignmentDto dto
        );

        Task<bool> DeleteAssignmentAsync(int id);
    }
}
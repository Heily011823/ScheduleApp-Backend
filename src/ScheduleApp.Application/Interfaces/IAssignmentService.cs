using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task SaveAssignmentAsync(Assignment assignment);
        Task<List<Assignment>> GetAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(int id);
        Task<Assignment?> UpdateAssignmentAsync(int id, Assignment assignment);
        Task<bool> DeleteAssignmentAsync(int id);
    }
}

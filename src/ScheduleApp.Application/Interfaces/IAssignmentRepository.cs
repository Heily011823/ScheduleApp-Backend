using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentRepository
    {
        Task CreateAsync(Assignment assignment);
        Task<List<Assignment>> GetAllAsync();
        Task<Assignment?> GetByIdAsync(int id);
        Task UpdateAsync(Assignment assignment);
        Task<bool> DeleteAsync(int id);
    }
}

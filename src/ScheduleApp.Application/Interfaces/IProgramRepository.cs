using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IProgramRepository
    {
        Task<List<Program>> GetAllAsync();
        Task<Program?> GetByIdAsync(Guid id);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
        Task CreateAsync(Program program);
        Task UpdateAsync(Program program);
        Task<bool> DeleteAsync(Guid id);
    }
}

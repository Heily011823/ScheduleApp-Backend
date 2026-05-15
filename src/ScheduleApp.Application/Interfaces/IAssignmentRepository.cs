using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentRepository
    {
        Task CreateAsync(Assignment assignment);

        Task<List<Assignment>> GetAllAsync();
    }
}
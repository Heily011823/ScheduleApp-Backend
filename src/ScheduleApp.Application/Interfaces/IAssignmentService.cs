using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task SaveAssignmentAsync(Assignment assignment);

        Task<List<Assignment>> GetAssignmentsAsync();
    }
}
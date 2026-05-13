using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(Guid id);

    Task<List<Subject>> GetActiveAsync();

    Task UpdateAsync(Subject subject);
}
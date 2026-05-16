using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(Guid id);

    Task<List<Subject>> GetActiveAsync();

    Task UpdateAsync(Subject subject);

    Task CreateAsync(Subject subject);

    Task<Subject?> GetByCodeAsync(string code);

    Task<List<Subject>> SearchAsync(
    string? search,
    int? semester,
    bool? isActive
    );
}
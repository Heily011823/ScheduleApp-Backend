using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectRestrictionRepository
{
    Task CreateAsync(SubjectRestriction restriction);

    Task<List<SubjectRestriction>> GetBySubjectIdAsync(Guid subjectId);

    Task DeleteBySubjectIdAsync(Guid subjectId);
}
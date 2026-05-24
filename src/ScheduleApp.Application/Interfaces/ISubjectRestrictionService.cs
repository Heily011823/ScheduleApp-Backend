using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectRestrictionService
{
    Task CreateRestrictionAsync(
        CreateSubjectRestrictionDto dto);

    Task<List<SubjectRestriction>>
        GetRestrictionsBySubjectIdAsync(Guid subjectId);

    Task DeleteRestrictionsBySubjectIdAsync(Guid subjectId);
}
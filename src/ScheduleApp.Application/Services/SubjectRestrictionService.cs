using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services;

public class SubjectRestrictionService
    : ISubjectRestrictionService
{
    private readonly ISubjectRestrictionRepository
        _repository;

    public SubjectRestrictionService(
        ISubjectRestrictionRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateRestrictionAsync(
        CreateSubjectRestrictionDto dto)
    {
        var restriction = new SubjectRestriction
        {
            Id = Guid.NewGuid(),

            SubjectId = dto.SubjectId,

            Day = dto.Day,

            StartTime = dto.StartTime,

            EndTime = dto.EndTime,

            RestrictionType = dto.RestrictionType,

            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(restriction);
    }

    public async Task<List<SubjectRestriction>>
        GetRestrictionsBySubjectIdAsync(Guid subjectId)
    {
        return await _repository
            .GetBySubjectIdAsync(subjectId);
    }

    public async Task DeleteRestrictionsBySubjectIdAsync(
        Guid subjectId)
    {
        await _repository
            .DeleteBySubjectIdAsync(subjectId);
    }
}
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectService
{
    Task<bool> DeleteSubjectAsync(Guid id);

    Task CreateSubjectAsync(CreateSubjectDto dto);

    Task UpdateSubjectAsync(Guid id, UpdateSubjectDto dto);
}
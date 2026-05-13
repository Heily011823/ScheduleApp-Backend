namespace ScheduleApp.Application.Interfaces;

public interface ISubjectService
{
    Task<bool> DeleteSubjectAsync(Guid id);
}
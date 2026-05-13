using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface IMateriaRepository
{
    Task<Materia?> GetByIdAsync(int id);
    Task<List<Materia>> GetActivasAsync();
    Task UpdateAsync(Materia materia);
}
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IClassroomService
    {
        Task<List<Classroom>> GetClassroomsAsync();

        Task<Classroom?> GetClassroomByIdAsync(Guid id);    

        Task CreateClassroomAsync(Classroom classroom);

        Task UpdateClassroomAsync(Classroom classroom);

        Task DeleteClassroomAsync(Guid id);                  

        Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);  
    }
}
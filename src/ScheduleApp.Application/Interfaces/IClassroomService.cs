using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Interfaces
{

    /*
     * Author: Salome Carmona
     * Feature: Classroom CRUD
     * Description: Service interface for classroom business logic
     */

    public interface IClassroomService
    {
        Task<List<Classroom>> GetClassroomsAsync();

        Task<Classroom?> GetClassroomByIdAsync(int id);

        Task CreateClassroomAsync(Classroom classroom);

        Task UpdateClassroomAsync(Classroom classroom);

        Task DeleteClassroomAsync(int id);
    }
}

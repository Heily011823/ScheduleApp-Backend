using ScheduleApp.Domain.Entities;
<<<<<<< HEAD
using ScheduleApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleApp.Application.Interfaces;


public interface IClassroomService
{
    Task<List<Classroom>> GetClassroomsAsync();

    Task<Classroom?> GetClassroomByIdAsync(Guid id);

    Task CreateClassroomAsync(Classroom classroom);

    Task UpdateClassroomAsync(Classroom classroom);

  
    Task DeleteClassroomAsync(Guid id);

 
    Task<Classroom?> ChangeStatusAsync(Guid id, bool isActive);
=======

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
>>>>>>> b3085647cd120c5e717d5b48bc1a47e5317e077c
}
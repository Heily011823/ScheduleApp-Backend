using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Services
{
    /*
      * Author: Salome Carmona
      * Feature: Classroom CRUD
      * Description: Handles classroom business logic
      */

    public class ClassroomService : IClassroomService
    {
        private readonly IClassroomRepository _repository;

        public ClassroomService(IClassroomRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Classroom>> GetClassroomsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Classroom?> GetClassroomByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateClassroomAsync(Classroom classroom)
        {
            await _repository.CreateAsync(classroom);
        }

        public async Task UpdateClassroomAsync(Classroom classroom)
        {
            await _repository.UpdateAsync(classroom);
        }

        public async Task DeleteClassroomAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}

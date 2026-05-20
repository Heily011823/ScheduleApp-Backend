using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    /*
     * Author: Salome Carmona
     * Feature: CRUD Program Semesters
     * Description: Handles program semester operations
     */

    public class ProgramSemesterService
    {
        private readonly IProgramSemesterRepository _repository;

        public ProgramSemesterService(IProgramSemesterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProgramSemester>> GetByProgramIdAsync(Guid programId)
        {
            return await _repository.GetByProgramIdAsync(programId);
        }

        public async Task UpdateAsync(List<ProgramSemester> semesters)
        {
            await _repository.UpdateAsync(semesters);
        }
    }
}
using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Interfaces
{

    /*
     * Author: Salome Carmona
     * Feature: Program Semesters CRUD
     * Description: Repository contract for program semesters
     */

    public interface IProgramSemesterRepository
    {
        Task<List<ProgramSemester>> GetByProgramIdAsync(Guid programId);

        Task UpdateAsync(List<ProgramSemester> semesters);
    }
}

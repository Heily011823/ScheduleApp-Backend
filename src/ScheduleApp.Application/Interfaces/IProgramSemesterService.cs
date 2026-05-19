using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Interfaces
{
    /*
    * Author: Salome Carmona
    * Feature: Program Semesters CRUD
    * Description: Service interface for managing program semesters
    */

    public interface IProgramSemesterService
    {
        Task<List<ProgramSemester>> GetByProgramIdAsync(Guid academicProgramId);

        Task UpdateAsync(List<ProgramSemester> semesters);
    }
}

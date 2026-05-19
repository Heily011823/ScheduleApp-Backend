using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories
{
    /*
     * Author: Salome Carmona
     * Feature: Program Semesters CRUD
     * Description: Repository for querying and updating program semesters
     */
    public class ProgramSemesterRepository : IProgramSemesterRepository
    {
        private readonly AppDbContext _context;

        public ProgramSemesterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProgramSemester>> GetByProgramIdAsync(Guid academicProgramId)
        {
            return await _context.ProgramSemesters
                .Where(ps => ps.AcademicProgramId == academicProgramId)
                .OrderBy(ps => ps.SemesterNumber)
                .ToListAsync();
        }

        public async Task UpdateAsync(List<ProgramSemester> semesters)
        {
            _context.ProgramSemesters.UpdateRange(semesters);

            await _context.SaveChangesAsync();
        }
    }
}
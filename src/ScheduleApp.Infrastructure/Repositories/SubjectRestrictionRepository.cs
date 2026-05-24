using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories;

public class SubjectRestrictionRepository
    : ISubjectRestrictionRepository
{
    private readonly AppDbContext _context;

    public SubjectRestrictionRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(
        SubjectRestriction restriction)
    {
        await _context.SubjectRestrictions.AddAsync(
            restriction);

        await _context.SaveChangesAsync();
    }

    public async Task<List<SubjectRestriction>>
        GetBySubjectIdAsync(Guid subjectId)
    {
        return await _context.SubjectRestrictions
            .Where(sr => sr.SubjectId == subjectId)
            .ToListAsync();
    }

    public async Task DeleteBySubjectIdAsync(
        Guid subjectId)
    {
        var restrictions =
            await _context.SubjectRestrictions
                .Where(sr => sr.SubjectId == subjectId)
                .ToListAsync();

        _context.SubjectRestrictions.RemoveRange(
            restrictions);

        await _context.SaveChangesAsync();
    }
}
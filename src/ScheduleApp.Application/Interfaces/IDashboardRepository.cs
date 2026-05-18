using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    public interface IDashboardRepository
    {
        DashboardSummaryDto GetSummary();
    }
}
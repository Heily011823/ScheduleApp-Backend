using ScheduleApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Services
{
    public class DashboardService
    {
        public DashboardSummaryDto GetSummary()
        {
            return new DashboardSummaryDto
            {
                Subjects = 12,
                Teachers = 8,
                Schedules = 20,
                Programs = 5,
                Classrooms = 15,
                Coordinators = 2
            };
        }
    }
}

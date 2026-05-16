using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.DTOs
{/// <summary>
 /// DTO used to return dashboard summary totals.
 /// </summary>
 /// <author>Salome Carmona</author>
    public class DashboardSummaryDto
    {
        public int Subjects { get; set; }

        public int Teachers { get; set; }

        public int Schedules { get; set; }

        public int Programs { get; set; }

        public int Classrooms { get; set; }

        public int Coordinators { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.DTOs
{
    public class SeedResultDto
    {
        public int Added { get; set; }
        public int Skipped { get; set; }
        public int Total { get; set; }
    }
}

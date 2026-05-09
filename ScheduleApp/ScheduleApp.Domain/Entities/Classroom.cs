using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Domain.Entities
{
    public class Classroom
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Building { get; set; }

        public int Floor { get; set; }

        public string Status { get; set; }

        public int Capacity { get; set; }

        public string Type { get; set; }
    }
}

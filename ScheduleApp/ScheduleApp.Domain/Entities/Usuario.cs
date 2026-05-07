using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public Rol Rol { get; set; }
    }
}

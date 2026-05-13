using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Interfaces
{
    public interface IMateriaService
    {
        Task<bool> EliminarMateriaAsync(int id);
    }
}

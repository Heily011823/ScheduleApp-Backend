using System;
using System.Collections.Generic;

namespace ScheduleApp.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Relación: Un rol tiene muchos usuarios
    public ICollection<User> Users { get; set; } = new List<User>();
}
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.DTOs;

/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: 174-Especialidades

public class SpecialtyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}



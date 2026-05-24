using System;
using System.Collections.Generic;
using System.Text;



using System;
using System.ComponentModel.DataAnnotations;


/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: 174-Especialidades

namespace ScheduleApp.Application.DTOs;

public class SpecialtyDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre de la especialidad es requerido")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public class CreateSpecialtyDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateSpecialtyDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}


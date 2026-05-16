using System;
using System.Collections.Generic;
using System.Text;

// src/ScheduleApp.Application/DTOs/TeacherResponseDto.cs
namespace ScheduleApp.Application.DTOs;

/// Autor: Mateo Quintero
/// Version: 0.2
/// Rama: 96-Crud-docentes
/// <summary>
/// DTO de respuesta con los datos públicos de un docente.
/// Es lo que retorna la API en todas las operaciones.
/// </summary>
public class TeacherResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentityDocument { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
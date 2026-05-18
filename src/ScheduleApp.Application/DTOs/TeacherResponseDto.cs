using System;

namespace ScheduleApp.Application.DTOs;

public class TeacherResponseDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string IdentityDocument { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    // ======================================================
    // NUEVAS PROPIEDADES
    // ======================================================

    public string Specialties { get; set; } = string.Empty;

    public int TeachingHours { get; set; }

    public string ContractType { get; set; } = string.Empty;

    // ======================================================

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
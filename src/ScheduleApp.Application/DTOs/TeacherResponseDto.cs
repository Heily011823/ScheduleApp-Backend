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

    public Guid? SpecialtyId { get; set; }
    public string? SpecialtyName { get; set; }

    // ======================================================
    // NUEVAS PROPIEDADES
    // ======================================================

    public int TeachingHours { get; set; }

    public string ContractType { get; set; } = string.Empty;

    // ======================================================

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }


    public List<SpecialtyDto> Specialties { get; set; } = new List<SpecialtyDto>();

    // ⚠️ Obsoleto: mantener por compatibilidad, pero marcar como obsoleto
    [Obsolete("Use Specialties instead")]
    public string SpecialtiesString => string.Join(", ", Specialties.Select(s => s.Name));
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services;



/// Autor:  Mateo Quintero 
/// Version: 0.1
/// rama: develop

/// <summary>
/// Implementación del servicio de especialidades.
/// Encargado de toda la lógica de negocio relacionada con especialidades.
/// </summary>
public class SpecialtyService : ISpecialtyService
{
    private readonly ISpecialtyRepository _specialtyRepository;

    public SpecialtyService(ISpecialtyRepository specialtyRepository)
    {
        _specialtyRepository = specialtyRepository;
    }

    /// <summary>
    /// Obtiene todas las especialidades activas
    /// </summary>
    public async Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync()
    {
        var specialties = await _specialtyRepository.GetAllSpecialtiesAsync();

        return specialties.Select(s => new SpecialtyDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            IsActive = s.IsActive,
            DisplayOrder = s.DisplayOrder
        });
    }

    /// <summary>
    /// Obtiene una especialidad por su ID
    /// </summary>
    public async Task<SpecialtyDto?> GetSpecialtyByIdAsync(Guid id)
    {
        var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
        if (specialty == null) return null;

        return new SpecialtyDto
        {
            Id = specialty.Id,
            Name = specialty.Name,
            Description = specialty.Description,
            IsActive = specialty.IsActive,
            DisplayOrder = specialty.DisplayOrder
        };
    }

    /// <summary>
    /// Obtiene una especialidad por su nombre
    /// </summary>
    public async Task<SpecialtyDto?> GetSpecialtyByNameAsync(string name)
    {
        var specialty = await _specialtyRepository.GetSpecialtyByNameAsync(name);
        if (specialty == null) return null;

        return new SpecialtyDto
        {
            Id = specialty.Id,
            Name = specialty.Name,
            Description = specialty.Description,
            IsActive = specialty.IsActive,
            DisplayOrder = specialty.DisplayOrder
        };
    }

    /// <summary>
    /// Crea una nueva especialidad
    /// </summary>
    public async Task<SpecialtyDto> CreateSpecialtyAsync(CreateSpecialtyDto dto)
    {
        // Verificar si ya existe una especialidad con el mismo nombre
        var exists = await _specialtyRepository.SpecialtyExistsAsync(dto.Name);
        if (exists)
            throw new InvalidOperationException($"Ya existe una especialidad con el nombre '{dto.Name}'.");

        var specialty = new Specialty
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description ?? string.Empty,
            DisplayOrder = dto.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _specialtyRepository.AddSpecialtyAsync(specialty);

        return new SpecialtyDto
        {
            Id = specialty.Id,
            Name = specialty.Name,
            Description = specialty.Description,
            IsActive = specialty.IsActive,
            DisplayOrder = specialty.DisplayOrder
        };
    }

    /// <summary>
    /// Actualiza una especialidad existente
    /// </summary>
    public async Task<SpecialtyDto?> UpdateSpecialtyAsync(Guid id, UpdateSpecialtyDto dto)
    {
        var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
        if (specialty == null)
            return null;

        // Verificar que el nuevo nombre no exista en otra especialidad
        var existingWithName = await _specialtyRepository.GetSpecialtyByNameAsync(dto.Name);
        if (existingWithName != null && existingWithName.Id != id)
            throw new InvalidOperationException($"Ya existe otra especialidad con el nombre '{dto.Name}'.");

        specialty.Name = dto.Name;
        specialty.Description = dto.Description ?? string.Empty;
        specialty.IsActive = dto.IsActive;
        specialty.DisplayOrder = dto.DisplayOrder;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _specialtyRepository.UpdateSpecialtyAsync(specialty);

        return new SpecialtyDto
        {
            Id = specialty.Id,
            Name = specialty.Name,
            Description = specialty.Description,
            IsActive = specialty.IsActive,
            DisplayOrder = specialty.DisplayOrder
        };
    }

    /// <summary>
    /// Elimina (desactiva) una especialidad
    /// </summary>
    public async Task<bool> DeleteSpecialtyAsync(Guid id)
    {
        var specialty = await _specialtyRepository.GetSpecialtyByIdAsync(id);
        if (specialty == null)
            return false;

        // Verificar si hay docentes asociados a esta especialidad
        var hasTeachers = await _specialtyRepository.HasTeachersWithSpecialtyAsync(id);
        if (hasTeachers)
            throw new InvalidOperationException(
                "No se puede eliminar la especialidad porque hay docentes asociados a ella. " +
                "Primero reasigne o desactive esos docentes.");

        specialty.IsActive = false;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _specialtyRepository.UpdateSpecialtyAsync(specialty);
        return true;
    }

    /// <summary>
    /// Valida que una lista de IDs de especialidades existan
    /// </summary>
    public async Task ValidateSpecialtyIdsExistAsync(IEnumerable<Guid> specialtyIds)
    {
        if (specialtyIds == null || !specialtyIds.Any())
            return;

        var existingSpecialties = await _specialtyRepository.GetAllSpecialtiesAsync();
        var existingIds = existingSpecialties.Select(s => s.Id).ToHashSet();

        var invalidIds = specialtyIds.Where(id => !existingIds.Contains(id)).ToList();

        if (invalidIds.Any())
        {
            var validIds = string.Join(", ", existingIds.Select(id => id.ToString()).Take(5));
            throw new InvalidOperationException(
                $"Las siguientes especialidades no existen: {string.Join(", ", invalidIds)}.\n" +
                $"Para ver las especialidades disponibles, use GET /api/teachers/metadata/specialties.\n" +
                $"IDs válidas (primeras 5): {validIds}...");
        }
    }

    /// <summary>
    /// Carga especialidades por defecto en la base de datos (solo usar una vez)
    /// </summary>
    public async Task<SeedResultDto> SeedSpecialtiesAsync(IEnumerable<object> defaultSpecialties)
    {
        var result = new SeedResultDto();

        foreach (var spec in defaultSpecialties)
        {
            var name = spec.GetType().GetProperty("Name")?.GetValue(spec)?.ToString();
            var description = spec.GetType().GetProperty("Description")?.GetValue(spec)?.ToString();
            var displayOrder = (int)(spec.GetType().GetProperty("DisplayOrder")?.GetValue(spec) ?? 0);

            if (string.IsNullOrEmpty(name)) continue;

            // Verificar si ya existe
            var exists = await _specialtyRepository.SpecialtyExistsAsync(name);
            if (exists)
            {
                result.Skipped++;
                continue;
            }

            // Crear nueva especialidad
            var specialty = new Specialty
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description ?? string.Empty,
                DisplayOrder = displayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _specialtyRepository.AddSpecialtyAsync(specialty);
            result.Added++;
        }

        result.Total = result.Added + result.Skipped;
        return result;
    }
}
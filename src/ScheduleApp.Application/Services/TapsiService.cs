using System;
using System.Collections.Generic;
using System.Text;

/// Autor:  Mateo Quintero 
/// Version: 0.2
/// rama: 33-Reglas-tapsi



// ScheduleApp.Application/Services/TapsiService.cs
namespace ScheduleApp.Application.Services;

using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

/// <summary>
/// Servicio que gestiona las reglas especiales TAPSI y valida
/// asignaciones de horario contra dichas reglas.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.1
public class TapsiService
{
    private readonly ITapsiRuleRepository _tapsiRuleRepository;
    private readonly ISubjectRepository _subjectRepository;

    public TapsiService(
        ITapsiRuleRepository tapsiRuleRepository,
        ISubjectRepository subjectRepository)
    {
        _tapsiRuleRepository = tapsiRuleRepository;
        _subjectRepository = subjectRepository;
    }

    /// <summary>
    /// Retorna todas las reglas TAPSI registradas en el sistema.
    /// </summary>
    public async Task<IEnumerable<TapsiRuleResponseDto>> GetAllRulesAsync()
    {
        var rules = await _tapsiRuleRepository.GetAllAsync();
        return rules.Select(MapToResponseDto);
    }

    /// <summary>
    /// Retorna una regla TAPSI por su ID.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Si la regla no existe.</exception>
    public async Task<TapsiRuleResponseDto> GetRuleByIdAsync(Guid id)
    {
        var rule = await _tapsiRuleRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Regla TAPSI con ID {id} no encontrada.");
        return MapToResponseDto(rule);
    }

    /// <summary>
    /// Crea una nueva regla TAPSI.
    /// </summary>
    public async Task<TapsiRuleResponseDto> CreateRuleAsync(TapsiRuleDto dto)
    {
        var rule = new TapsiRule
        {
            Id = Guid.NewGuid(),
            RuleType = dto.RuleType.ToUpper().Trim(),
            Description = dto.Description.Trim(),
            Value = dto.Value.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _tapsiRuleRepository.AddAsync(rule);
        return MapToResponseDto(rule);
    }

    /// <summary>
    /// Actualiza una regla TAPSI existente.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Si la regla no existe.</exception>
    public async Task<TapsiRuleResponseDto> UpdateRuleAsync(Guid id, TapsiRuleDto dto)
    {
        var rule = await _tapsiRuleRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Regla TAPSI con ID {id} no encontrada.");

        rule.RuleType = dto.RuleType.ToUpper().Trim();
        rule.Description = dto.Description.Trim();
        rule.Value = dto.Value.Trim();
        rule.IsActive = dto.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        await _tapsiRuleRepository.UpdateAsync(rule);
        return MapToResponseDto(rule);
    }

    /// <summary>
    /// Elimina una regla TAPSI por su ID.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Si la regla no existe.</exception>
    public async Task DeleteRuleAsync(Guid id)
    {
        var rule = await _tapsiRuleRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Regla TAPSI con ID {id} no encontrada.");
        await _tapsiRuleRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Valida una asignación de horario contra las reglas TAPSI activas.
    /// Solo aplica si la materia está marcada como IsTapsi = true.
    /// Retorna advertencias por cada regla que no se cumpla.
    /// </summary>
    public async Task<TapsiValidationResultDto> ValidateAssignmentAsync(
        TapsiValidationRequestDto request)
    {
        var result = new TapsiValidationResultDto { IsValid = true };

        // Verificar si la materia es TAPSI
        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
        if (subject is null)
        {
            result.IsValid = false;
            result.Warnings.Add("La materia especificada no existe.");
            return result;
        }

        // Si la materia no es TAPSI, no aplican las reglas
        if (!subject.IsTapsi)
        {
            result.IsValid = true;
            return result;
        }

        // Obtener reglas activas y validar cada una
        var activeRules = await _tapsiRuleRepository.GetActiveAsync();

        foreach (var rule in activeRules)
        {
            var warning = ValidateRule(rule, request);
            if (warning is not null)
            {
                result.IsValid = false;
                result.Warnings.Add(warning);
            }
        }

        return result;
    }

    /// <summary>
    /// Evalúa una regla individual contra los datos de la asignación.
    /// Retorna un mensaje de advertencia si la regla no se cumple, o null si se cumple.
    /// </summary>
    private string? ValidateRule(TapsiRule rule, TapsiValidationRequestDto request)
    {
        switch (rule.RuleType)
        {
            case "MAX_DAILY_HOURS":
                // Valida que las horas diarias no superen el máximo permitido
                if (int.TryParse(rule.Value, out int maxHours))
                {
                    var start = TimeSpan.Parse(request.StartTime);
                    var end = TimeSpan.Parse(request.EndTime);
                    var hours = (end - start).TotalHours;
                    if (hours > maxHours)
                        return $"⚠️ TAPSI: La asignación supera el máximo de {maxHours} horas diarias permitidas (tiene {hours}h).";
                }
                break;

            case "ALLOWED_DAYS":
                // Valida que el día esté en la lista de días permitidos
                var allowedDays = rule.Value.Split(',')
                    .Select(d => d.Trim().ToLower()).ToList();
                if (!allowedDays.Contains(request.Day.ToLower()))
                    return $"⚠️ TAPSI: El día '{request.Day}' no está permitido. Días válidos: {rule.Value}.";
                break;

            case "TIME_RANGE":
                // Valida que el horario esté dentro del rango permitido
                var parts = rule.Value.Split('-');
                if (parts.Length == 2)
                {
                    var rangeStart = TimeSpan.Parse(parts[0].Trim());
                    var rangeEnd = TimeSpan.Parse(parts[1].Trim());
                    var assignStart = TimeSpan.Parse(request.StartTime);
                    var assignEnd = TimeSpan.Parse(request.EndTime);

                    if (assignStart < rangeStart || assignEnd > rangeEnd)
                        return $"⚠️ TAPSI: El horario {request.StartTime}-{request.EndTime} está fuera del rango permitido {rule.Value}.";
                }
                break;

            case "MAX_WEEKLY_HOURS":
                // Nota: validación completa requiere consultar todas las asignaciones
                // de la semana. Esta validación básica alerta al coordinador.
                return $"⚠️ TAPSI: Verifique que el total semanal no supere {rule.Value} horas (regla MAX_WEEKLY_HOURS).";
        }

        return null;
    }

    /// <summary>Mapea una entidad TapsiRule a su DTO de respuesta.</summary>
    private TapsiRuleResponseDto MapToResponseDto(TapsiRule rule) =>
        new()
        {
            Id = rule.Id,
            RuleType = rule.RuleType,
            Description = rule.Description,
            Value = rule.Value,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt
        };
}

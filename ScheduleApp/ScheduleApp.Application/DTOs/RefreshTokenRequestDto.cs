using System;

// ScheduleApp.Application/DTOs/RefreshTokenRequestDto.cs
namespace ScheduleApp.Application.DTOs;

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

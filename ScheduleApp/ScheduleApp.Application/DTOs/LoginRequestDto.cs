using System;

// ScheduleApp.Application/DTOs/LoginRequestDto.cs
namespace ScheduleApp.Application.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

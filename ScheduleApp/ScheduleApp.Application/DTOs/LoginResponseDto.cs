using System;

// ScheduleApp.Application/DTOs/LoginResponseDto.cs
namespace ScheduleApp.Application.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

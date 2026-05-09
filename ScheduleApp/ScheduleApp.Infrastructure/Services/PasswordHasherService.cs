using System;

namespace ScheduleApp.Infrastructure.Services;

using ScheduleApp.Application.Interfaces;

public class PasswordHasherService : IPasswordHasher
{
    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);
}
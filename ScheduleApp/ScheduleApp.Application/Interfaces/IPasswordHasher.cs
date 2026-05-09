using System;

// ScheduleApp.Application/Interfaces/IPasswordHasher.cs
namespace ScheduleApp.Application.Interfaces;

public interface IPasswordHasher
{
    bool Verify(string password, string hash);
    string Hash(string password);
}
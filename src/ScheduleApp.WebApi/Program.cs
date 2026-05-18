using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Infrastructure.Data;
using ScheduleApp.Infrastructure.Repositories;
using ScheduleApp.Infrastructure.Services;
using System;
using System.Text;

// Cargar variables de entorno utilizando DotNetEnv
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Leer variables desde appsettings.json o appsettings.Development.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Validaciones de seguridad
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Falta la cadena de conexion 'DefaultConnection' en el appsettings."
    );
}

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException(
        "Falta la variable 'Jwt:Secret' en el appsettings."
    );
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "Falta la variable 'Jwt:Issuer' en el appsettings."
    );
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Falta la variable 'Jwt:Audience' en el appsettings."
    );
}

// =========================================================================
// REGISTRO DE SERVICIOS
// =========================================================================

// Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ScheduleApp API",
        Version = "v1"
    });
});

// =========================================================================
// MÓDULO DE AULAS
// =========================================================================

builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<IClassroomAvailabilityService, AvailabilityService>();

// =========================================================================
// MÓDULO DE PROGRAMAS / SEMESTRES
// =========================================================================

builder.Services.AddScoped<IProgramSemesterRepository, ProgramSemesterRepository>();
builder.Services.AddScoped<ProgramSemesterService>();

// =========================================================================
// MÓDULO DE DOCENTES
// =========================================================================

builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherService, TeacherService>();

// =========================================================================
// REGLAS TAPSI
// =========================================================================

builder.Services.AddScoped<ITapsiRuleRepository, TapsiRuleRepository>();
builder.Services.AddScoped<TapsiService>();

// =========================================================================
// MÓDULO DE USUARIOS Y AUTENTICACIÓN
// =========================================================================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IJwtService>(
    provider => new JwtService(jwtSecret, jwtIssuer, jwtAudience)
);

builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<AuthService>();

// =========================================================================
// MÓDULO DE MATERIAS Y ASIGNACIONES
// =========================================================================

builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

// =========================================================================
// MÓDULO DE GENERACIÓN AUTOMÁTICA DE HORARIOS
// =========================================================================

builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

builder.Services.AddScoped<
    IScheduleGenerationService,
    ScheduleApp.Application.Services.ScheduleGenerationService
>();

// =========================================================================
// DASHBOARD
// =========================================================================

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<DashboardService>();

// =========================================================================
// AUTENTICACIÓN JWT
// =========================================================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// =========================================================================
// PIPELINE HTTP
// =========================================================================

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleApp API V1");
    options.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
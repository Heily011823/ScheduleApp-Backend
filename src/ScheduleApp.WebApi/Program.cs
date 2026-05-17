using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Infrastructure.Data;
using ScheduleApp.Infrastructure.Repositories;
using ScheduleApp.Infrastructure.Services;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Leer variables desde appsettings.json o appsettings.Development.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Validaciones de seguridad
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection' en el appsettings.");

if (string.IsNullOrWhiteSpace(jwtSecret))
    throw new InvalidOperationException("Falta la variable 'Jwt:Secret' en el appsettings.");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("Falta la variable 'Jwt:Issuer' en el appsettings.");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Falta la variable 'Jwt:Audience' en el appsettings.");

// ==========================================
// Módulo de Aulas (Modificado y Corregido)
// ==========================================
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();

// CORREGIDO: Ahora apunta exactamente a la clase concreta 'AvailabilityService' que tienes en tu proyecto
builder.Services.AddScoped<IClassroomAvailabilityService, AvailabilityService>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Controllers
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

// Módulo de Docentes
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherService, TeacherService>();

// Reglas TAPSI
builder.Services.AddScoped<ITapsiRuleRepository, TapsiRuleRepository>();
builder.Services.AddScoped<TapsiService>();

// Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ClockSkew = TimeSpan.Zero
        };
    });

// Inyección de dependencias de Usuarios y Auth
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService>(provider => new JwtService(jwtSecret, jwtIssuer, jwtAudience));
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<AuthService>();

// Módulo de Materias y Asignaciones
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

var app = builder.Build();

// Configuración del Pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleApp API V1");
    options.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de controladores de la API
app.MapControllers();

app.Run();
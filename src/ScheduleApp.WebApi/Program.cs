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

// Validaciones de seguridad para garantizar la presencia de variables críticas
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection' en el appsettings.");

if (string.IsNullOrWhiteSpace(jwtSecret))
    throw new InvalidOperationException("Falta la variable 'Jwt:Secret' en el appsettings.");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("Falta la variable 'Jwt:Issuer' en el appsettings.");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Falta la variable 'Jwt:Audience' en el appsettings.");

// =========================================================================
// REGISTRO DE SERVICIOS (CONTENEDOR DE INYECCIÓN DE DEPENDENCIAS)
// =========================================================================

// Módulo de Aulas (Configurado y Corregido)
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<IClassroomAvailabilityService, AvailabilityService>();

// Infraestructura de Persistencia (Base de Datos SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Controladores de la API
builder.Services.AddControllers();

// Documentación de la API con Swagger
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

// Autenticación basada en Tokens JWT
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

// Módulo de Usuarios y Autenticación (Auth)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService>(provider => new JwtService(jwtSecret, jwtIssuer, jwtAudience));
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<AuthService>();

// Módulo de Horarios con Validación de Créditos
builder.Services.AddScoped<IProgramSemesterRepository, ProgramSemesterRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<CreditValidationService>();


// Módulo de Materias y Asignaciones (Soportan Paginación Eficiente)
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

var app = builder.Build();





// =========================================================================
// CONFIGURACIÓN DEL PIPELINE DE PETICIONES HTTP (MIDDLEWARES)
// =========================================================================

// Activar interfaz gráfica de Swagger en entornos de desarrollo y pruebas
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleApp API V1");
    options.RoutePrefix = "swagger";
});

// Seguridad: Validar quién es el usuario (AuthN) y a qué tiene permiso (AuthZ)
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de los controladores de la API hacia sus rutas correspondientes
app.MapControllers();

// Arrancar la aplicación web
app.Run();
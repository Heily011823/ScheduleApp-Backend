using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Infrastructure.Data;
using ScheduleApp.Infrastructure.Repositories;
using ScheduleApp.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// CONFIGURACIÓN
// ======================================================
var connectionString =
    builder.Configuration
        .GetConnectionString(
            "DefaultConnection");

// En tu contenedor de DI (Program.cs o un archivo de extensión)
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();

var jwtSecret =
    builder.Configuration["Jwt:Secret"];

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    builder.Configuration["Jwt:Audience"];

// ======================================================
// VALIDACIONES
// ======================================================
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Falta la cadena de conexión.");
}

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException(
        "Falta Jwt:Secret.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "Falta Jwt:Issuer.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Falta Jwt:Audience.");
}

// ======================================================
// DB CONTEXT
// ======================================================
builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlServer(connectionString)
);

// ======================================================
// CONTROLADORES
// ======================================================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new()
        {
            Title = "ScheduleApp API",
            Version = "v1"
        });
});

// ======================================================
// AULAS
// ======================================================
builder.Services.AddScoped<
    IClassroomRepository,
    ClassroomRepository>();

builder.Services.AddScoped<
    IClassroomService,
    ClassroomService>();

builder.Services.AddScoped<
    IClassroomAvailabilityService,
    AvailabilityService>();

// ======================================================
// PROGRAMAS / SEMESTRES
// ======================================================
builder.Services.AddScoped<
    IProgramSemesterRepository,
    ProgramSemesterRepository>();

builder.Services.AddScoped<
    ProgramSemesterService>();

// ======================================================
// DOCENTES
// ======================================================
builder.Services.AddScoped<
    ITeacherRepository,
    TeacherRepository>();

builder.Services.AddScoped<
    ITeacherService,
    TeacherService>();

// ======================================================
// TAPSI
// ======================================================
builder.Services.AddScoped<
    ITapsiRuleRepository,
    TapsiRuleRepository>();

builder.Services.AddScoped<
    TapsiService>();

// ======================================================
// USUARIOS
// ======================================================
builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();

builder.Services.AddScoped<
    IUserService,
    UserService>();

builder.Services.AddScoped<IJwtService>(
    provider =>
        new JwtService(
            jwtSecret,
            jwtIssuer,
            jwtAudience));

builder.Services.AddScoped<
    IPasswordHasher,
    PasswordHasherService>();

builder.Services.AddScoped<
    AuthService>();

// ======================================================
// MATERIAS
// ======================================================
builder.Services.AddScoped<
    ISubjectRepository,
    SubjectRepository>();

builder.Services.AddScoped<
    ISubjectService,
    SubjectService>();

// ======================================================
// RESTRICCIONES DE MATERIAS
// ======================================================
builder.Services.AddScoped<
    ISubjectRestrictionRepository,
    SubjectRestrictionRepository>();

builder.Services.AddScoped<
    ISubjectRestrictionService,
    SubjectRestrictionService>();

// ======================================================
// ASIGNACIONES
// ======================================================
builder.Services.AddScoped<
    IAssignmentRepository,
    AssignmentRepository>();

builder.Services.AddScoped<
    IAssignmentService,
    AssignmentService>();

// ======================================================
// HORARIOS
// ======================================================
builder.Services.AddScoped<
    IScheduleRepository,
    ScheduleRepository>();

builder.Services.AddScoped<
    IScheduleGenerationService,
    ScheduleGenerationService>();

builder.Services.AddScoped<
    IScheduleService,
    ScheduleGenerationService>();

// ======================================================
// EXPORTACIONES
// ======================================================
builder.Services.AddScoped<
    IPdfExportService,
    PdfExportService>();

builder.Services.AddScoped<
    IExcelExportService,
    ExcelExportService>();

// ======================================================
// DASHBOARD
// ======================================================
builder.Services.AddScoped<
    IDashboardRepository,
    DashboardRepository>();

builder.Services.AddScoped<
    DashboardService>();

// ======================================================
// VALIDACIÓN DE CRÉDITOS
// ======================================================
builder.Services.AddScoped<
    CreditValidationService>();

// ======================================================
// JWT
// ======================================================
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)),

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ClockSkew = TimeSpan.Zero
            };
    });

// ======================================================
// BUILD
// ======================================================
var app = builder.Build();

// ======================================================
// PIPELINE HTTP
// ======================================================
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "ScheduleApp API V1");

    options.RoutePrefix = "swagger";
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
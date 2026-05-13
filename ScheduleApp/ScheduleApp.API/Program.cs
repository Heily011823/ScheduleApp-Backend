// ScheduleApp.API/Program.cs

/// Autor: Mateo Quintero
/// Version: 0.1

// Punto de entrada de la aplicacion. Configura servicios,
// middleware, autenticacion JWT e inyeccion de dependencias.

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
builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();
builder.Services.AddScoped<IMateriaService, MateriaService>();

// Base de datos
// Registra AppDbContext con SQL Server usando la cadena de conexion
// definida en appsettings.Development.json: DefaultConnection.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticacion JWT
// Configura el esquema Bearer para validar tokens JWT en cada request.
// Parametros leidos desde appsettings.json, seccion Jwt:
//   Secret: clave para firmar/verificar el token, minimo 32 caracteres.
//   Issuer: emisor del token, debe coincidir al validar.
//   Audience: destinatario del token, debe coincidir al validar.
//   ClockSkew = Zero: sin margen de tolerancia en la expiracion.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

// Inyeccion de dependencias
// Registra las implementaciones concretas para cada interfaz de la capa Application.
// Scope significa una instancia por request HTTP.
builder.Services.AddScoped<IUserRepository, UserRepository>();       // acceso a BD para usuarios
builder.Services.AddScoped<IJwtService, JwtService>();               // generacion de tokens JWT
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>(); // hashing con BCrypt
builder.Services.AddScoped<AuthService>();                           // logica de autenticacion

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();
builder.Services.AddScoped<IMateriaService, MateriaService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migraciones automaticas
// Al iniciar la app aplica automaticamente las migraciones pendientes.
// Util en desarrollo para no tener que correr dotnet ef database update manualmente.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Orden obligatorio: primero Authentication, luego Authorization.
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Caesar API", Version = "v1" });

    // Configuración para el botón Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT con el prefijo 'Bearer' (ejemplo: Bearer tu_token_aqui)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Agregar DbContext para MySQL
builder.Services.AddDbContext<caesar.data.CaesarDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<caesar.data.CaesarMessageRepository>();
builder.Services.AddScoped<caesar.service.CaesarService>();

// Configuración JWT
var key = builder.Configuration["Jwt:Key"] ?? "clave_super_secreta_123456789";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "caesar-api";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddAuthorization();

// Agregar configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware para restringir acceso solo a la IP pública 187.155.101.200 (usando X-Forwarded-For)
app.Use(async (context, next) =>
{
    var allowedIp = "187.155.101.200";
    var remoteIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                   ?? context.Connection.RemoteIpAddress?.ToString();
    Console.WriteLine($"IP detectada (X-Forwarded-For): {remoteIp}");
    if (remoteIp != allowedIp)
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Forbidden: Solo permitido desde la IP pública de la escuela.");
        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Caesar API V1");
});

app.UseHttpsRedirection();

// Usar CORS antes de autenticación y autorización
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CaesarEncryptController : ControllerBase
{
    // ...
}

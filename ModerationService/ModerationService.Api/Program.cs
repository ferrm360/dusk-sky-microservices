using Microsoft.EntityFrameworkCore;
using ModerationService.Api.Infrastructure;
using ModerationService.Api.Endpoints;
using ModerationService.Api.Repositories.Interfaces;
using ModerationService.Api.Repositories.Implementations;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Logging detallado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configuración para escuchar en 0.0.0.0:80
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Configuración de PostgreSQL
string? host = Environment.GetEnvironmentVariable("MODERATION_DB_HOST") ?? "localhost";
string? port = Environment.GetEnvironmentVariable("MODERATION_DB_PORT") ?? "5432";
string? database = Environment.GetEnvironmentVariable("MODERATION_DB_NAME") ?? "moderationdb";
string? user = Environment.GetEnvironmentVariable("MODERATION_DB_USER") ?? "postgres";
string? password = Environment.GetEnvironmentVariable("MODERATION_DB_PASSWORD") ?? "supersecret";

if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("❌ Faltan variables de entorno para la base de datos.");
    throw new InvalidOperationException("Faltan variables de entorno.");
}

// 📚 PostgreSQL Connection String
string connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";
Console.WriteLine($"🔗 Conectando a PostgreSQL en {host}:{port}...");

builder.Services.AddDbContext<ModerationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositorios
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ISanctionRepository, SanctionRepository>();

// Configuración JSON
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Moderation Service API",
        Version = "v1"
    });
    options.AddServer(new OpenApiServer { Url = "/moderation" }); // 🔥 Para que Swagger use el prefijo NGINX
});

var app = builder.Build();

// 🔥 Activar Developer Exception Page
app.UseDeveloperExceptionPage();

// Swagger y UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/moderation/swagger/v1/swagger.json", "Moderation Service API v1");
    options.RoutePrefix = "swagger";
});

// Endpoints simples
app.MapGet("/", () => "ModerationService is running!");
app.MapGet("/moderation", () => "ModerationService via NGINX!");

// Endpoints de Reportes y Sanciones
var moderationGroup = app.MapGroup("/moderation");
moderationGroup.MapReportEndpoints();
moderationGroup.MapSanctionEndpoints();

// 🚀 Migraciones automáticas con reintentos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();

    int maxRetries = 10;
    int delayMilliseconds = 3000;

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            Console.WriteLine($"⏳ Intentando aplicar migraciones a PostgreSQL (intento {attempt}/{maxRetries})...");
            db.Database.Migrate();
            Console.WriteLine("✅ Migraciones aplicadas exitosamente.");
            break; // Salir del bucle si se logra
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            Console.WriteLine($"⚠️ Error al aplicar migraciones: {ex.Message}. Reintentando en {delayMilliseconds / 1000.0} segundos...");
            await Task.Delay(delayMilliseconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fallo al aplicar migraciones después de {maxRetries} intentos: {ex.Message}");
            throw; // Si falla en el último intento, propagar el error
        }
    }
}

Console.WriteLine("🚀 Moderation Service iniciado y esperando conexiones...");
app.Run();

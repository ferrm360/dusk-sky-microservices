using Microsoft.EntityFrameworkCore;
using ModerationService.Api.Infrastructure;
using ModerationService.Api.Endpoints;
using ModerationService.Api.Repositories.Interfaces;
using ModerationService.Api.Repositories.Implementations;
using ModerationService.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

string? host     = Environment.GetEnvironmentVariable("MODERATION_DB_HOST");
string? port     = Environment.GetEnvironmentVariable("MODERATION_DB_PORT") ?? "3306";
string? database = Environment.GetEnvironmentVariable("MODERATION_DB_NAME");
string? user     = Environment.GetEnvironmentVariable("MODERATION_DB_USER");
string? password = Environment.GetEnvironmentVariable("MODERATION_DB_PASSWORD");

if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
{
    throw new InvalidOperationException("Faltan variables de entorno para la conexión a la base de datos.");
}

string connectionString = $"server={host};port={port};user={user};password={password};database={database}";

builder.Services.AddDbContext<ModerationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ISanctionRepository, SanctionRepository>();
builder.Services.AddScoped<ReportController>();
builder.Services.AddScoped<SanctionController>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "ModerationService is running!");
app.MapReportEndpoints();
app.MapSanctionEndpoints();

app.Run();

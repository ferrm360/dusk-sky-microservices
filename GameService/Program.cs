using GameService.Repositories.Interfaces;
using GameService.Repositories.Implementations;
using GameService.Services.Interfaces;
using GameService.Services.Implementations;
using GameService.Settings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using GameService.Utils;

var builder = WebApplication.CreateBuilder(args);

// Configuración
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Configuración MongoDB
builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = builder.Configuration["MONGODB_URI_GAME"]!;
    options.DatabaseName = builder.Configuration["MONGODB_DB_GAME"]!;
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(config.ConnectionString);
});

// Servicios y Repositorios
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IGameDetailsRepository, GameDetailsRepository>();
builder.Services.AddScoped<IGameService, GameService.Services.Implementations.GameService>();
builder.Services.AddHostedService<DatabaseSeeder>();

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🚨 CORS abierto
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    hostingContext.HostingEnvironment.EnvironmentName = "Development";
});

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll"); // Aplica la política CORS aquí

app.MapControllers();
app.Run();

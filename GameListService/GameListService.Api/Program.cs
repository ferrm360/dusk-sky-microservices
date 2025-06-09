using GameListService.Api.Infrastructure;
using GameListService.Api.Repositories.Interfaces;
using GameListService.Api.Repositories.Implementations;
using GameListService.Api.Services.Interfaces;
using GameListService.Api.Services.Implementations;
using GameListService.Api.Endpoints;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Configuración de Logging detallado
builder.Logging.ClearProviders();           // Opcional: limpia los proveedores por defecto
builder.Logging.AddConsole();               // Habilita logs a consola
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Nivel de detalle (puedes cambiar a Information si quieres menos ruido)

// Configurar URLs (no cambiar)
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Endpoints y Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GameList Service API",
        Version = "v1"
    });
    options.AddServer(new OpenApiServer { Url = "/" });
});

// Configuración JSON
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configuración MongoDB con Logging explícito
var mongoSettings = builder.Configuration.GetSection("MongoSettings");
string? dbName = mongoSettings["DatabaseName"];
string? user = Environment.GetEnvironmentVariable("GAMELIST_MONGO_USER");
string? password = Environment.GetEnvironmentVariable("GAMELIST_MONGO_PASSWORD");
string? host = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "mongodb";

if (string.IsNullOrWhiteSpace(dbName) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("❌ Faltan variables de entorno para MongoDB. Revisa GAMELIST_MONGO_DATABASE, USER, PASSWORD.");
    throw new InvalidOperationException("Faltan variables de entorno para conectar con MongoDB.");
}

string connectionString = $"mongodb://{user}:{password}@{host}:27017/{dbName}?authSource={dbName}";
Console.WriteLine($"🔗 Conectando a MongoDB en {connectionString}...");
var connector = new MongoConnector();
var database = await connector.ConnectWithRetriesAsync(connectionString, dbName);
Console.WriteLine("✅ Conexión exitosa a MongoDB.");
builder.Services.AddSingleton<IMongoDatabase>(database);

// Repositorios y servicios
builder.Services.AddScoped<IGameListRepository, GameListRepository>();
builder.Services.AddScoped<IGameListItemRepository, GameListItemRepository>();
builder.Services.AddScoped<IGameListManager, GameListManager>();

var app = builder.Build();

// 🔥 Habilitar DeveloperExceptionPage para ver errores detallados
if (app.Environment.IsDevelopment() || true) // O usa solo "if (true)" para forzar siempre
{
    app.UseDeveloperExceptionPage();
}

// Habilitar Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/lists/swagger/v1/swagger.json", "GameList Service API v1");
    options.RoutePrefix = "swagger";
});

// Endpoint de prueba
app.MapGet("/", () => "GameListService is running (root)");
// Endpoint simple para verificar servicio
app.MapGet("/lists", () => "GameListService is running!");


// Mapear Endpoints
app.MapGameListEndpoints();
app.MapGameListItemEndpoints();

Console.WriteLine("🚀 GameList Service iniciado y esperando conexiones...");

app.Run();

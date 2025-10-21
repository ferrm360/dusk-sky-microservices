using GameListService.Api.Infrastructure;
using GameListService.Api.Models;
using GameListService.Api.Repositories.Implementations;
using GameListService.Api.Repositories.Interfaces;
using GameListService.Api.Services.Implementations;
using GameListService.Api.Services.Interfaces;
using GameListService.Api.Endpoints;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Logging detallado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configurar para escuchar en 0.0.0.0:80
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
    options.AddServer(new OpenApiServer { Url = "/lists" });
});

// Configuración JSON
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configuración MongoDB
string? dbName = Environment.GetEnvironmentVariable("GAMELIST_MONGO_DATABASE"); // GameListService
string? user = Environment.GetEnvironmentVariable("GAMELIST_MONGO_USER");        // gamelistuser
string? password = Environment.GetEnvironmentVariable("GAMELIST_MONGO_PASSWORD");  // securepassword456
string? host = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "mongodb";

if (string.IsNullOrWhiteSpace(dbName) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("❌ Faltan variables de entorno para MongoDB. Revisa GAMELIST_MONGO_DATABASE, USER, PASSWORD.");
    throw new InvalidOperationException("Faltan variables de entorno para conectar con MongoDB.");
}

// 🎯 CAMBIO CLAVE 1: Usar dbName como authSource.
string authSource = dbName; 

// 🎯 CAMBIO CLAVE 2: Forzar el mecanismo de autenticación SCRAM-SHA-256.
string connStr = $"mongodb://{user}:{password}@{host}:27017/{dbName}?authSource={authSource}&authMechanism=SCRAM-SHA-256";

Console.WriteLine($"🔗 Conectando a MongoDB en {connStr}...");
var connector = new MongoConnector();
var database = await connector.ConnectWithRetriesAsync(connStr, dbName);
Console.WriteLine("✅ Conexión exitosa a MongoDB.");

builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddSingleton<IGameListRepository, GameListRepository>();
builder.Services.AddSingleton<IGameListItemRepository, GameListItemRepository>();
builder.Services.AddSingleton<IGameListManager, GameListManager>();

var app = builder.Build();

// 🔥 Activar página de errores en modo dev (opcional: quítalo en prod)
app.UseDeveloperExceptionPage();

// Swagger y UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/lists/swagger/v1/swagger.json", "GameList Service API v1");
    options.RoutePrefix = "swagger"; 
});

// Endpoint simple de prueba
app.MapGet("/", () => "GameListService is running!");

// Endpoints CRUD
app.MapGameListEndpoints();

Console.WriteLine("🚀 GameListService iniciado y esperando conexiones...");

app.Run();
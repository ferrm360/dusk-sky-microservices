using System.Text.Json.Serialization;
using AddService.Api.Endpoints;
using AddService.Api.Infrastructure;
using AddService.Api.Repositories;
using AddService.Api.Services;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Escuchar en 0.0.0.0:80 (útil para contenedores)
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ad Service API",
        Version = "v1",
        Description = "Servicio para manejar y servir anuncios con prioridad aleatoria"
    });
    options.AddServer(new OpenApiServer { Url = "/ads" }); // si montas en /ads
});

// JSON
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// 🔐 MongoDB desde variables de entorno
string? dbName = Environment.GetEnvironmentVariable("AD_MONGO_DATABASE");
string? user = Environment.GetEnvironmentVariable("AD_MONGO_USER");
string? password = Environment.GetEnvironmentVariable("AD_MONGO_PASSWORD");
string? host = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "mongodb";

if (string.IsNullOrWhiteSpace(dbName) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("❌ Faltan variables de entorno para MongoDB. Revisa AD_MONGO_DATABASE, USER, PASSWORD.");
    throw new InvalidOperationException("Faltan variables de entorno para conectar con MongoDB.");
}

string connStr = $"mongodb://{user}:{password}@{host}:27017/{dbName}?authSource=admin";
Console.WriteLine($"🔗 Conectando a MongoDB en {connStr}...");

var connector = new MongoConnector();
var database = await connector.ConnectWithRetriesAsync(connStr, dbName);
Console.WriteLine("✅ Conexión exitosa a MongoDB.");

// Inyecciones
builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();

var app = builder.Build();

// Dev tools
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/ads/swagger/v1/swagger.json", "Ad Service API v1");
    options.RoutePrefix = "swagger"; // expone en /ads/swagger
});

// Endpoints
app.MapAdvertisementEndpoints();

Console.WriteLine("🚀 AdService iniciado y escuchando...");
app.Run();

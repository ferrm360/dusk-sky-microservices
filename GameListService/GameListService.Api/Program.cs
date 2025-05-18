using GameListService.Api.Infrastructure;
using GameListService.Api.Repositories.Interfaces;
using GameListService.Api.Repositories.Implementations;
using GameListService.Api.Services.Interfaces;
using GameListService.Api.Services.Implementations;
using GameListService.Api.Endpoints;
using MongoDB.Driver;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var mongoSettings = builder.Configuration.GetSection("MongoSettings");
string? dbName = mongoSettings["DatabaseName"];
string? user = Environment.GetEnvironmentVariable("GAMELIST_MONGO_USER");
string? password = Environment.GetEnvironmentVariable("GAMELIST_MONGO_PASSWORD");
string? host = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "mongodb";

string connectionString = $"mongodb://{user}:{password}@{host}:27017/{dbName}?authSource={dbName}";

if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(dbName))
{
    throw new InvalidOperationException("Faltan variables de entorno para conectar con MongoDB.");
}


var connector = new MongoConnector();
var database = await connector.ConnectWithRetriesAsync(connectionString, dbName);
builder.Services.AddSingleton<IMongoDatabase>(database);

builder.Services.AddScoped<IGameListRepository, GameListRepository>();
builder.Services.AddScoped<IGameListItemRepository, GameListItemRepository>();
builder.Services.AddScoped<IGameListManager, GameListManager>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "GameListService is running!");
app.MapGameListEndpoints();
app.MapGameListItemEndpoints();

app.Run();

using CommentService.Api.Infraestructure;
using CommentService.Api.Models;
using CommentService.Api.Repositories.Interfaces;
using CommentService.Api.Repositories.Implementations;
using CommentService.Api.Services.Interfaces;
using CommentService.Api.Services.Implementations;
using CommentService.Api.Endpoints;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoSettings = builder.Configuration.GetSection("MongoSettings");
string? databaseName = mongoSettings.GetValue<string?>("DatabaseName");

string? mongoUser = Environment.GetEnvironmentVariable("COMMENT_MONGO_USER");
string? mongoPassword = Environment.GetEnvironmentVariable("COMMENT_MONGO_PASSWORD");
string? mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "mongodb"; 
string connectionString = $"mongodb://{mongoUser}:{mongoPassword}@{mongoHost}:27017/{databaseName}?authSource={databaseName}";

if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException("Faltan variables de entorno o configuración de Mongo.");
}

var mongoConnector = new MongoConnector();
var database = await mongoConnector.ConnectWithRetriesAsync(connectionString, databaseName);

builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddSingleton<ICommentRepository, CommentRepository>();
builder.Services.AddSingleton<ICommentManager, CommentManager>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "CommentService API is running!");
app.MapCommentEndpoints();

app.Run();

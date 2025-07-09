using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

// Configuración de entorno y archivos
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Configuración MongoDB
builder.Services.Configure<MongoSettings>(options =>
{
    options.ConnectionString = builder.Configuration["USER_GAME_TRACKING_MONGO_URI"]!;
    options.DatabaseName = builder.Configuration["USER_GAME_TRACKING_MONGO_DB"]!;
    options.CollectionName = "GameTrackings";
});

// Serializador Guid para evitar errores con MongoDB
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Cliente MongoDB singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(config.ConnectionString);
});

// Inyección de dependencias
builder.Services.AddScoped<IGameTrackingRepository, GameTrackingRepository>();
builder.Services.AddScoped<IGameTrackingService, GameTrackingService>();

// Web API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    hostingContext.HostingEnvironment.EnvironmentName = "Development";
});

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

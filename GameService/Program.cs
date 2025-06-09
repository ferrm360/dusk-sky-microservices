using GameService.Repositories.Interfaces;
using GameService.Repositories.Implementations;
using GameService.Services.Interfaces;
using GameService.Services.Implementations;
using GameService.Settings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using GameService.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

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

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IGameDetailsRepository, GameDetailsRepository>();
builder.Services.AddScoped<IGameService, GameService.Services.Implementations.GameService>();
builder.Services.AddHostedService<DatabaseSeeder>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    hostingContext.HostingEnvironment.EnvironmentName = "Development";
});



app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

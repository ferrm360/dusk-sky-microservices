using GameService.Models;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Text.Json;

namespace GameService.Utils;

public class DatabaseSeeder : IHostedService
{
    private readonly IMongoDatabase _db;
    private readonly IWebHostEnvironment _env;

    public DatabaseSeeder(IMongoClient client, IWebHostEnvironment env)
    {
        _db = client.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_DB_GAME")!);
        _env = env;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var gamesCollection = _db.GetCollection<Game>("games");
        if (await gamesCollection.CountDocumentsAsync(FilterDefinition<Game>.Empty) > 0)
            return;

        Console.WriteLine("[SEED] Poblando base de datos de GameService...");

        string rootPath = Path.Combine(_env.ContentRootPath, "data");

        await SeedCollection<Game>("games", Path.Combine(rootPath, "gameDB.games.json"));
        await SeedCollection<GameDetails>("game_details", Path.Combine(rootPath, "gameDB.game_details.json"));
        await SeedCollection<GameImage>("game_images", Path.Combine(rootPath, "gameDB.game_images.json"));
        await SeedCollection<Genre>("genres", Path.Combine(rootPath, "gameDB.genres.json"));
    }

    private async Task SeedCollection<T>(string collectionName, string filePath)
    {
        var collection = _db.GetCollection<T>(collectionName);

        if (!File.Exists(filePath)) return;

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items?.Any() == true)
        {
            await collection.InsertManyAsync(items);
            Console.WriteLine($"[SEED] Insertados {items.Count} documentos en '{collectionName}'");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

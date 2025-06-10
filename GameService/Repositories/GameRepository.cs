// Repositories/Implementations/GameRepository.cs
using GameService.Models;
using GameService.Repositories.Interfaces;
using GameService.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameService.Repositories.Implementations
{
    public class GameRepository : IGameRepository
    {
        private readonly IMongoCollection<Game> _games;

        public GameRepository(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _games = database.GetCollection<Game>("games");
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await _games.Find(_ => true).ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            return await _games.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Game?> GetBySteamAppIdAsync(int steamAppId)
        {
            return await _games.Find(g => g.SteamAppId == steamAppId).FirstOrDefaultAsync();
        }

        public async Task AddAsync(Game game)
        {
            await _games.InsertOneAsync(game);
        }

        public async Task<List<Game>> SearchByNameAsync(string partialName)
        {
            var filter = Builders<Game>.Filter.Regex(g => g.Name, new MongoDB.Bson.BsonRegularExpression(partialName, "i"));
            return await _games.Find(filter).ToListAsync();
        }

    }
}

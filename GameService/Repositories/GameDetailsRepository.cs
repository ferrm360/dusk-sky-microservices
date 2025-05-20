// Repositories/Implementations/GameDetailsRepository.cs
using GameService.Models;
using GameService.Repositories.Interfaces;
using GameService.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameService.Repositories.Implementations
{
    public class GameDetailsRepository : IGameDetailsRepository
    {
        private readonly IMongoCollection<GameDetails> _details;

        public GameDetailsRepository(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _details = database.GetCollection<GameDetails>("game_details");
        }

        public async Task<GameDetails?> GetByGameIdAsync(Guid gameId)
        {
            return await _details.Find(d => d.GameId == gameId).FirstOrDefaultAsync();
        }

        public async Task AddAsync(GameDetails gameDetails)
        {
            await _details.InsertOneAsync(gameDetails);
        }
    }
}

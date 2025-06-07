using GameListService.Api.Models;
using GameListService.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace GameListService.Api.Repositories.Implementations
{
    public class GameListRepository : IGameListRepository
    {
        private readonly IMongoCollection<GameList> _collection;

        public GameListRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<GameList>("game_lists");
        }

        public async Task<IEnumerable<GameList>> GetAllByUserAsync(string userId)
        {
            var filter = Builders<GameList>.Filter.Eq(g => g.UserId, userId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<GameList?> GetByIdAsync(string id)
        {
            var filter = Builders<GameList>.Filter.Eq(g => g.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(GameList list)
        {
            await _collection.InsertOneAsync(list);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<GameList>.Filter.Eq(g => g.Id, id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        public async Task<bool> UpdateAsync(GameList list)
        {
            var filter = Builders<GameList>.Filter.Eq(l => l.Id, list.Id);
            var update = Builders<GameList>.Update
                .Set(l => l.Name, list.Name)
                .Set(l => l.Description, list.Description)
                .Set(l => l.IsPublic, list.IsPublic);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

    }
}

using GameListService.Api.Models;
using GameListService.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace GameListService.Api.Repositories.Implementations
{
    public class GameListItemRepository : IGameListItemRepository
    {
        private readonly IMongoCollection<GameListItem> _collection;

        public GameListItemRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<GameListItem>("game_list_items");
        }

        public async Task<IEnumerable<GameListItem>> GetItemsByListIdAsync(string listId)
        {
            var filter = Builders<GameListItem>.Filter.Eq(i => i.ListId, listId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task AddItemAsync(GameListItem item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task<bool> DeleteItemAsync(string itemId)
        {
            var filter = Builders<GameListItem>.Filter.Eq(i => i.Id, itemId);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        public async Task<bool> UpdateItemAsync(GameListItem item)
        {
            var filter = Builders<GameListItem>.Filter.Eq(i => i.Id, item.Id);
            var update = Builders<GameListItem>.Update
                .Set(i => i.Comment, item.Comment);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

    }
}

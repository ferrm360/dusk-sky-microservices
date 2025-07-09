using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class GameTrackingRepository : IGameTrackingRepository
{
    private readonly IMongoCollection<GameTracking> _collection;

    public GameTrackingRepository(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<GameTracking>(settings.Value.CollectionName);
    }

    public async Task<List<GameTracking>> GetByUserIdAsync(string userId) =>
        await _collection.Find(t => t.UserId == userId).ToListAsync();

    public async Task<GameTracking?> GetAsync(Guid id) =>
        await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();

    public async Task AddAsync(GameTracking tracking) =>
        await _collection.InsertOneAsync(tracking);

    public async Task UpdateAsync(GameTracking tracking) =>
        await _collection.ReplaceOneAsync(t => t.Id == tracking.Id, tracking);

    public async Task DeleteAsync(Guid id) =>
        await _collection.DeleteOneAsync(t => t.Id == id);

    public async Task<List<string>> GetGameIdsByStatusAsync(string userId, string status)
    {
        var filter = Builders<GameTracking>.Filter.Eq(t => t.UserId, userId) &
                     Builders<GameTracking>.Filter.Eq(t => t.Status, status);

        return await _collection
            .Find(filter)
            .Project(t => t.GameId)
            .ToListAsync();
    }

    public async Task<List<string>> GetLikedGameIdsAsync(string userId)
    {
        var filter = Builders<GameTracking>.Filter.Eq(t => t.UserId, userId) &
                     Builders<GameTracking>.Filter.Eq(t => t.Liked, true);

        return await _collection
            .Find(filter)
            .Project(t => t.GameId)
            .ToListAsync();
    }

    public async Task<GameTracking?> GetByUserAndGameAsync(string userId, string gameId)
    {
        var filter = Builders<GameTracking>.Filter.Eq(t => t.UserId, userId) &
                     Builders<GameTracking>.Filter.Eq(t => t.GameId, gameId);

        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

}
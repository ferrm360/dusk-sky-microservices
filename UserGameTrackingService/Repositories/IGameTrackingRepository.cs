public interface IGameTrackingRepository
{
    Task<List<GameTracking>> GetByUserIdAsync(string userId);
    Task<GameTracking?> GetAsync(Guid id);
    Task AddAsync(GameTracking tracking);
    Task UpdateAsync(GameTracking tracking);
    Task DeleteAsync(Guid id);
    Task<List<string>> GetGameIdsByStatusAsync(string userId, string status);
    Task<List<string>> GetLikedGameIdsAsync(string userId);
    Task<GameTracking?> GetByUserAndGameAsync(string userId, string gameId);


}
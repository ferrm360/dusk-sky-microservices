public interface IGameTrackingService
{
    Task<List<GameTracking>> GetTrackingsByUserAsync(string userId);
    Task<GameTracking?> GetByIdAsync(Guid id);
    Task<GameTracking> AddAsync(GameTrackingDTO dto);
    Task<bool> UpdateAsync(Guid id, GameTrackingDTO dto);
    Task<bool> DeleteAsync(Guid id);
    Task<List<string>> GetGameIdsByStatusAsync(string userId, string status);
    Task<List<string>> GetLikedGameIdsAsync(string userId);
    Task<GameTracking?> GetByUserAndGameAsync(string userId, string gameId);

}
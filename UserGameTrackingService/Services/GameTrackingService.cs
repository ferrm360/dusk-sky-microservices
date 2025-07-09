public class GameTrackingService : IGameTrackingService
{
    private readonly IGameTrackingRepository _repository;

    public GameTrackingService(IGameTrackingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GameTracking>> GetTrackingsByUserAsync(string userId) =>
        await _repository.GetByUserIdAsync(userId);

    public async Task<GameTracking?> GetByIdAsync(Guid id) =>
        await _repository.GetAsync(id);

    public async Task<GameTracking> AddAsync(GameTrackingDTO dto)
{
    var tracking = new GameTracking
    {
        Id = Guid.NewGuid(),
        UserId = dto.UserId,
        GameId = dto.GameId,
        Status = string.IsNullOrWhiteSpace(dto.Status) ? null : dto.Status,
        Liked = dto.Liked ?? false,
        UpdatedAt = DateTime.UtcNow
    };

    await _repository.AddAsync(tracking);
    return tracking;
}

   public async Task<bool> UpdateAsync(Guid id, GameTrackingDTO dto)
{
    var existing = await _repository.GetAsync(id);
    if (existing is null)
        return false;

    existing.Status = string.IsNullOrWhiteSpace(dto.Status) ? null : dto.Status;
    existing.Liked = dto.Liked ?? false;
    existing.UpdatedAt = DateTime.UtcNow;

    await _repository.UpdateAsync(existing);
    return true;
}
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetAsync(id);
        if (existing is null)
            return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<List<string>> GetGameIdsByStatusAsync(string userId, string status) =>
        await _repository.GetGameIdsByStatusAsync(userId, status);

    public async Task<List<string>> GetLikedGameIdsAsync(string userId) =>
        await _repository.GetLikedGameIdsAsync(userId);
            
    public async Task<GameTracking?> GetByUserAndGameAsync(string userId, string gameId) =>
    await _repository.GetByUserAndGameAsync(userId, gameId);
        
    }
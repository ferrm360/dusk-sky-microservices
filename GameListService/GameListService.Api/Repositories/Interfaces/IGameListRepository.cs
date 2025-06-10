using GameListService.Api.Models;

namespace GameListService.Api.Repositories.Interfaces
{
    public interface IGameListRepository
    {
        Task<IEnumerable<GameList>> GetAllByUserAsync(string userId);
        Task<GameList?> GetByIdAsync(string id);
        Task CreateAsync(GameList list);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(GameList list);
        Task<bool> IncrementLikesAsync(string id);
        Task<bool> DecrementLikesAsync(string id);
        Task<IEnumerable<GameList>> GetMostLikedAsync(int limit = 10);
        Task<IEnumerable<GameList>> GetMostRecentAsync(int limit = 10);

    }
}
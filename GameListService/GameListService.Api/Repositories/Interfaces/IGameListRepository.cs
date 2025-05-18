namespace GameListService.Api.Repositories.Interfaces
{
    public interface IGameListRepository
    {
        Task<IEnumerable<GameList>> GetAllByUserAsync(string userId);
        Task<GameList?> GetByIdAsync(string id);
        Task CreateAsync(GameList list);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(GameList list);

    }
}
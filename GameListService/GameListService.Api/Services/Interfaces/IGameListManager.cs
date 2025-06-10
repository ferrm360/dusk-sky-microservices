using GameListService.Api.Models;

namespace GameListService.Api.Services.Interfaces
{
    public interface IGameListManager
    {
        Task<IEnumerable<GameList>> GetUserListsAsync(string userId);
        Task<GameList?> GetListByIdAsync(string listId);
        Task CreateListAsync(GameList list);
        Task<bool> UpdateListAsync(GameList list);
        Task<bool> DeleteListAsync(string listId);

        Task<IEnumerable<GameListItem>> GetItemsByListIdAsync(string listId);
        Task AddItemToListAsync(GameListItem item);
        Task<bool> UpdateItemAsync(GameListItem item);
        Task<bool> DeleteItemAsync(string itemId);
        Task<List<GameList?>> GetRecentListsAsync();
    }
}

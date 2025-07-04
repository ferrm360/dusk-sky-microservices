using GameListService.Api.Models;

namespace GameListService.Api.Services.Interfaces
{
    public interface IGameListManager
    {
        Task<IEnumerable<GameList>> GetUserListsAsync(string userId);
        Task<GameList?> GetListByIdAsync(string listId);
        Task<GameList> CreateListAsync(GameList list);
        Task<bool> UpdateListAsync(GameList list);
        Task<bool> DeleteListAsync(string listId);

        Task<IEnumerable<GameListItem>> GetItemsByListIdAsync(string listId);
        Task AddItemToListAsync(GameListItem item);
        Task<bool> UpdateItemAsync(GameListItem item);
        Task<bool> DeleteItemAsync(string itemId);
        
        Task<IEnumerable<GameList>> GetMostRecentListsAsync();
        Task<IEnumerable<GameList>> GetMostLikedListsAsync();
        Task<bool> LikeListAsync(string listId);
        Task<bool> UnlikeListAsync(string listId);
    }
}

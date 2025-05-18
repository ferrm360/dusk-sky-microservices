using System.Collections.Generic;
using System.Threading.Tasks;
using GameListService.Api.Models;
using System.Threading.Tasks;

namespace GameListService.Api.Repositories.Interfaces
{
    public interface IGameListItemRepository
    {
        Task<IEnumerable<GameListItem>> GetItemsByListIdAsync(string listId);
        Task AddItemAsync(GameListItem item);
        Task<bool> DeleteItemAsync(string itemId);
        Task<bool> UpdateItemAsync(GameListItem item);

    }
}
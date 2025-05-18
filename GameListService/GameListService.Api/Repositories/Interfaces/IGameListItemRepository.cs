namespace GameListService.Repositories.Interfaces
{
    public interface IGameListItemRepository
    {
        Task<IEnumerable<GameListItem>> GetItemsByListIdAsync(string listId);
        Task AddItemAsync(GameListItem item);
        Task<bool> DeleteItemAsync(string itemId);
        Task<bool> UpdateItemAsync(GameListItem item);

    }
}
using GameListService.Api.Models;
using GameListService.Api.Repositories.Interfaces;
using GameListService.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;


namespace GameListService.Api.Services.Implementations
{
    public class GameListManager : IGameListManager
    {
        private readonly IGameListRepository _listRepository;
        private readonly IGameListItemRepository _itemRepository;

        public GameListManager(
            IGameListRepository listRepository,
            IGameListItemRepository itemRepository)
        {
            _listRepository = listRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<GameList>> GetUserListsAsync(string userId)
            => await _listRepository.GetAllByUserAsync(userId);

        public async Task<GameList?> GetListByIdAsync(string listId)
            => await _listRepository.GetByIdAsync(listId);

        public async Task CreateListAsync(GameList list)
            => await _listRepository.CreateAsync(list);

        public async Task<bool> UpdateListAsync(GameList list)
            => await _listRepository.UpdateAsync(list);

        public async Task<bool> DeleteListAsync(string listId)
            => await _listRepository.DeleteAsync(listId);

        public async Task<IEnumerable<GameListItem>> GetItemsByListIdAsync(string listId)
            => await _itemRepository.GetItemsByListIdAsync(listId);

        public async Task AddItemToListAsync(GameListItem item)
            => await _itemRepository.AddItemAsync(item);

        public async Task<bool> UpdateItemAsync(GameListItem item)
            => await _itemRepository.UpdateItemAsync(item);

        public async Task<bool> DeleteItemAsync(string itemId)
            => await _itemRepository.DeleteItemAsync(itemId);

        public async Task<List<GameList?>> GetRecentListsAsync()
            => await _listRepository.GetRecentListsAsync();
    }
}

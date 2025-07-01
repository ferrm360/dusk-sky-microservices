// Repositories/Interfaces/IGameRepository.cs
using GameService.Models;

namespace GameService.Repositories.Interfaces
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(Guid id);
        Task<Game?> GetBySteamAppIdAsync(int steamAppId);
        Task AddAsync(Game game);  
        Task<List<Game>> SearchByNameAsync(string partialName);
  
                               
    }
}

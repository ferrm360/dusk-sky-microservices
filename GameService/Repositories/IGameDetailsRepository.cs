// Repositories/Interfaces/IGameDetailsRepository.cs
using GameService.Models;

namespace GameService.Repositories.Interfaces
{
    public interface IGameDetailsRepository
    {
        Task<GameDetails?> GetByGameIdAsync(Guid gameId);
        Task AddAsync(GameDetails gameDetails);
    }
}

using GameService.DTOs;
using GameService.Models;

namespace GameService.Services.Interfaces
{
    public interface IGameService
    {
        Task<List<GamePreviewDTO>> GetAllGamePreviewsAsync();
        Task<GameDetailsDTO?> GetFullGameDetailsAsync(Guid gameId);
        Task<Guid?> ImportGameFromSteamAsync(int steamAppId);
        Task<Game?> GetBySteamAppIdAsync(int steamAppId);


    }
}

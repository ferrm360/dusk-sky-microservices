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
        Task<GamePreviewDTO?> GetGamePreviewByIdAsync(Guid gameId);
        Task<List<GameDetailsDTO>> SearchGameDetailsByNameAsync(string name);
        Task<List<GamePreviewDTO>> SearchGamePreviewsByNameAsync(string name);

    }
}

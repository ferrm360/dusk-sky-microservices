using GameService.Models;

namespace GameService.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<GameImage?> GetByGameIdAsync(Guid gameId);
        Task<string?> GetRandomScreenshotAsync(Guid gameId);  
        Task AddAsync(GameImage image);

    }
}

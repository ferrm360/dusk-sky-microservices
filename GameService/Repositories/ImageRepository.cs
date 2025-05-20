// Repositories/Implementations/ImageRepository.cs
using GameService.Models;
using GameService.Repositories.Interfaces;
using GameService.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameService.Repositories.Implementations
{
    public class ImageRepository : IImageRepository
    {
        private readonly IMongoCollection<GameImage> _images;

        public ImageRepository(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _images = database.GetCollection<GameImage>("images");
        }

        public async Task<GameImage?> GetByGameIdAsync(Guid gameId)
        {
            return await _images.Find(i => i.GameId == gameId).FirstOrDefaultAsync();
        }

        public async Task<string?> GetRandomScreenshotAsync(Guid gameId)
        {
            var image = await GetByGameIdAsync(gameId);
            if (image?.Screenshots == null || image.Screenshots.Count == 0)
                return null;

            var rand = new Random();
            return image.Screenshots[rand.Next(image.Screenshots.Count)];
        }

        public async Task AddAsync(GameImage image)
        {       
            await _images.InsertOneAsync(image);
        }

        async Task<GameImage?> IImageRepository.GetByGameIdAsync(Guid gameId)
        {
            return await _images.Find(i => i.GameId == gameId).FirstOrDefaultAsync();
        }
    }
}

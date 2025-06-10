// Services/Implementations/GameService.cs
using System.Text.Json;
using GameService.DTOs;
using GameService.Models;
using GameService.Repositories.Interfaces;
using GameService.Services.Interfaces;

namespace GameService.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IGameDetailsRepository _detailsRepository;

        public GameService(
            IGameRepository gameRepository,
            IImageRepository imageRepository,
            IGameDetailsRepository detailsRepository)
        {
            _gameRepository = gameRepository;
            _imageRepository = imageRepository;
            _detailsRepository = detailsRepository;
        }

        public async Task<List<GamePreviewDTO>> GetAllGamePreviewsAsync()
        {
            var games = await _gameRepository.GetAllAsync();
            var previews = new List<GamePreviewDTO>();

            foreach (var game in games)
            {
                var image = await _imageRepository.GetByGameIdAsync(game.Id);


                previews.Add(new GamePreviewDTO
                {
                    Id = game.Id,
                    Name = game.Name,
                    HeaderUrl = image?.HeaderUrl ?? ""
                });
            }

            return previews;
        }

        public async Task<Game?> GetBySteamAppIdAsync(int steamAppId)
        {
            return await _gameRepository.GetBySteamAppIdAsync(steamAppId);
        }

        public async Task<GameDetailsDTO?> GetFullGameDetailsAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            var details = await _detailsRepository.GetByGameIdAsync(gameId);
            var images = await _imageRepository.GetByGameIdAsync(gameId);

            if (game == null || details == null)
                return null;

            var screenshots = images?.Screenshots ?? new List<string>();
            var headerUrl = images?.HeaderUrl ?? "";
            var randomScreenshot = screenshots.Count > 0
                ? screenshots[new Random().Next(screenshots.Count)]
                : null;

            return new GameDetailsDTO
            {
                Id = game.Id,
                Name = game.Name,
                Description = details.Description,
                Developer = details.Developer,
                Publisher = details.Publisher,
                ReleaseDate = details.ReleaseDate,
                Platforms = details.Platforms,
                Genres = details.Genres,
                HeaderUrl = headerUrl,
                RandomScreenshot = randomScreenshot,
                AllScreenshots = screenshots
            };
        }

        public async Task<GamePreviewDTO?> GetGamePreviewByIdAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null) return null;

            var image = await _imageRepository.GetByGameIdAsync(gameId);

            return new GamePreviewDTO
            {
                Id = game.Id,
                Name = game.Name,
                HeaderUrl = image?.HeaderUrl ?? ""
            };
        }


        public async Task<Guid?> ImportGameFromSteamAsync(int steamAppId)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://store.steampowered.com/api/appdetails?appids={steamAppId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var rawJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement.GetProperty(steamAppId.ToString());

            if (!root.GetProperty("success").GetBoolean())
                return null;

            var data = root.GetProperty("data");

            var id = Guid.NewGuid();

            await _gameRepository.AddAsync(new Game
            {
                Id = id,
                SteamAppId = steamAppId,
                Name = data.GetProperty("name").GetString()!,
            });

            await _detailsRepository.AddAsync(new GameDetails
            {
                Id = Guid.NewGuid(),
                GameId = id,
                Description = data.GetProperty("short_description").GetString() ?? "N/A",
                Developer = data.TryGetProperty("developers", out var devArr) && devArr.GetArrayLength() > 0
                    ? devArr[0].GetString()!
                    : "Unknown",
                Publisher = data.TryGetProperty("publishers", out var pubArr) && pubArr.GetArrayLength() > 0
                    ? pubArr[0].GetString()!
                    : "Unknown",
                ReleaseDate = data.GetProperty("release_date").GetProperty("date").GetString() ?? "Unknown",
                Platforms = new Dictionary<string, bool>
                {
                    ["windows"] = data.GetProperty("platforms").GetProperty("windows").GetBoolean(),
                    ["mac"] = data.GetProperty("platforms").GetProperty("mac").GetBoolean(),
                    ["linux"] = data.GetProperty("platforms").GetProperty("linux").GetBoolean()
                },
                Genres = data.TryGetProperty("genres", out var genres)
                    ? genres.EnumerateArray().Select(g => g.GetProperty("description").GetString()!).ToList()
                    : new List<string>()
            });

            // Insertar Images
            var screenshots = data.TryGetProperty("screenshots", out var ss)
                ? ss.EnumerateArray().Select(s => s.GetProperty("path_full").GetString()!).ToList()
                : new List<string>();

            await _imageRepository.AddAsync(new GameImage
            {
                Id = Guid.NewGuid(),
                GameId = id,
                HeaderUrl = data.GetProperty("header_image").GetString() ?? "",
                Screenshots = screenshots
            });

            return id;
        }

        public async Task<List<GameDetailsDTO>> SearchGameDetailsByNameAsync(string name)
        {
            var games = await _gameRepository.SearchByNameAsync(name);
            var result = new List<GameDetailsDTO>();

            foreach (var game in games)
            {
                var details = await _detailsRepository.GetByGameIdAsync(game.Id);
                var images = await _imageRepository.GetByGameIdAsync(game.Id);

                if (details == null)
                    continue;

                var screenshots = images?.Screenshots ?? new List<string>();
                var headerUrl = images?.HeaderUrl ?? "";
                var randomScreenshot = screenshots.Count > 0
                    ? screenshots[new Random().Next(screenshots.Count)]
                    : null;

                result.Add(new GameDetailsDTO
                {
                    Id = game.Id,
                    Name = game.Name,
                    Description = details.Description,
                    Developer = details.Developer,
                    Publisher = details.Publisher,
                    ReleaseDate = details.ReleaseDate,
                    Platforms = details.Platforms,
                    Genres = details.Genres,
                    HeaderUrl = headerUrl,
                    RandomScreenshot = randomScreenshot,
                    AllScreenshots = screenshots
                });
            }

            return result;
        }


        public async Task<List<GamePreviewDTO>> SearchGamePreviewsByNameAsync(string name)
        {
            var games = await _gameRepository.SearchByNameAsync(name);
            var previews = new List<GamePreviewDTO>();

            foreach (var game in games)
            {
                var image = await _imageRepository.GetByGameIdAsync(game.Id);

                previews.Add(new GamePreviewDTO
                {
                    Id = game.Id,
                    Name = game.Name,
                    HeaderUrl = image?.HeaderUrl ?? ""
                });
            }

            return previews;
        }


    }
}

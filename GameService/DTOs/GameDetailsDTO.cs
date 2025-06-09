// DTOs/GameDetailsDTO.cs
namespace GameService.DTOs
{
    public class GameDetailsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Developer { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public string ReleaseDate { get; set; } = null!;
        public Dictionary<string, bool> Platforms { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public string HeaderUrl { get; set; } = null!;
        public string? RandomScreenshot { get; set; }
        public List<string> AllScreenshots { get; set; } = new();
    }
}

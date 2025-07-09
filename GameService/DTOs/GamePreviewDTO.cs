namespace GameService.DTOs
{
    public class GamePreviewDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string HeaderUrl { get; set; } = null!;
    }
}

public class GameTrackingDTO
{
  public string UserId { get; set; } = null!;
  public string GameId { get; set; } = null!;
  public string? Status { get; set; }
  public bool? Liked { get; set; }
}
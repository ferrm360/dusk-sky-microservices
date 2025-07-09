using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class GameTracking
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = null!;

    [BsonElement("game_id")]
    public string GameId { get; set; } = null!;

    [BsonElement("status")]
    public string? Status { get; set; }

    [BsonElement("liked")]
    public bool Liked { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

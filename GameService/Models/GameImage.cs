using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GameService.Models
{
    public class GameImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("_id")]
        public Guid Id { get; set; }

        [BsonElement("game_id")]
        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("game_id")]
        public Guid GameId { get; set; }

        [BsonElement("header_url")]
        [JsonPropertyName("header_url")]
        public string HeaderUrl { get; set; } = null!;

        [BsonElement("screenshots")]
        [JsonPropertyName("screenshots")]
        public List<string> Screenshots { get; set; } = new();
    }
}

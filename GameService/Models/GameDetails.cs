using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GameService.Models
{
    public class GameDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("_id")]
        public Guid Id { get; set; }

        [BsonElement("game_id")]
        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("game_id")]
        public Guid GameId { get; set; }

        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [BsonElement("developer")]
        [JsonPropertyName("developer")]
        public string Developer { get; set; } = null!;

        [BsonElement("publisher")]
        [JsonPropertyName("publisher")]
        public string Publisher { get; set; } = null!;

        [BsonElement("release_date")]
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = null!;

        [BsonElement("platforms")]
        [JsonPropertyName("platforms")]
        public Dictionary<string, bool> Platforms { get; set; } = new();

        [BsonElement("genres")]
        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; } = new();
    }
}

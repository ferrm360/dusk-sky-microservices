using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GameService.Models
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [JsonPropertyName("_id")]
        public Guid Id { get; set; }

        [BsonElement("steam_appid")]
        [JsonPropertyName("steam_appid")]
        public int? SteamAppId { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}

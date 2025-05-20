using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameService.Models
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("steam_appid")]
        public int? SteamAppId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;
    }
}

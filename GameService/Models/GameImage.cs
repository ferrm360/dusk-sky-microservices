using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameService.Models
{
    public class GameImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("game_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid GameId { get; set; }

        [BsonElement("header_url")]
        public string HeaderUrl { get; set; } = null!;

        [BsonElement("screenshots")]
        public List<string> Screenshots { get; set; } = new();
    }
}

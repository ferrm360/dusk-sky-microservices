using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameService.Models
{
    public class GameDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("game_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid GameId { get; set; }

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("developer")]
        public string Developer { get; set; } = null!;

        [BsonElement("publisher")]
        public string Publisher { get; set; } = null!;

        [BsonElement("release_date")]
        public string ReleaseDate { get; set; } = null!;

        [BsonElement("platforms")]
        public Dictionary<string, bool> Platforms { get; set; } = new();

        [BsonElement("genres")]
        public List<string> Genres { get; set; } = new();


    }
}

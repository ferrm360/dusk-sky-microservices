using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameListService.Api.Models
{
    public class GameListItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("list_id")]
        public string ListId { get; set; } = string.Empty;

        [BsonElement("game_id")]
        public string GameId { get; set; } = string.Empty;

        [BsonElement("comment")]
        public string Comment { get; set; } = string.Empty;
    }
}

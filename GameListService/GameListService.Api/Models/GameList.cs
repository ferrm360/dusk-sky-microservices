using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GameListService.Api.Models
{
    public class GameList
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("is_public")]
        public bool IsPublic { get; set; } = true;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement("likes")]
         public int Likes { get; set; } = 0;
    }
}

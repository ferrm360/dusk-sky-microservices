// Models/Genre.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameService.Models
{
    public class Genre
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;
    }
}

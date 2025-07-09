using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AddService.Api.Models;

public class Advertisement
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("mediaUrl")]
    public string MediaUrl { get; set; } = string.Empty;

    [BsonElement("linkUrl")]
    public string? LinkUrl { get; set; }

    [BsonElement("priority")]
    public int Priority { get; set; } // 1 a 10

    [BsonElement("active")]
    public bool Active { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("validUntil")]
    public DateTime? ValidUntil { get; set; }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CommentService.Api.Enums;

namespace CommentService.Api.Models;

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("reviewId")]
    public string ReviewId { get; set; } = string.Empty;

    [BsonElement("authorId")]
    public string AuthorId { get; set; } = string.Empty;

    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;

    [BsonElement("date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public CommentStatus Status { get; set; } = CommentStatus.visible;
}

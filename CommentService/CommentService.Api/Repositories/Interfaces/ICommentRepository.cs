using CommentService.Api.Models;
using CommentService.Api.Enums;

namespace CommentService.Api.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetCommentByIdAsync(string id);
    Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId);
    Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(string authorId);
    Task CreateCommentAsync(Comment comment);
    Task<bool> UpdateCommentStatusAsync(string id, CommentStatus status);
    Task<bool> DeleteCommentAsync(string id);
    Task<IEnumerable<Comment>> GetAllCommentsAsync(); 
}

using CommentService.Api.Repositories.Interfaces;
using CommentService.Api.Models;
using CommentService.Api.Enums;
using CommentService.Api.Services.Interfaces;

namespace CommentService.Api.Services.Implementations
{
    public class CommentManager : ICommentManager
    {
        private readonly ICommentRepository _repo;
        public CommentManager(ICommentRepository repo) => _repo = repo;

        public async Task<Comment?> CreateCommentAsync(Comment comment)
        {
            await _repo.CreateCommentAsync(comment);
            return comment;
        }

        public Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId)
            => _repo.GetCommentsByReviewIdAsync(reviewId);

        public async Task<bool> UpdateCommentStatusAsync(string id, CommentStatus status)
            => await _repo.UpdateCommentStatusAsync(id, status);

        public async Task<bool> DeleteCommentAsync(string id)
            => await _repo.DeleteCommentAsync(id);
    }
}
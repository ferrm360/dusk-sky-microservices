using MongoDB.Driver;
using CommentService.Api.Models;
using CommentService.Api.Repositories.Interfaces;
using CommentService.Api.Enums;

namespace CommentService.Api.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentRepository(IMongoDatabase database)
        {
            _comments = database.GetCollection<Comment>("comments");
        }

        public async Task<Comment?> GetCommentByIdAsync(string id)
        {
            return await _comments.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId)
        {
            return await _comments.Find(c => c.ReviewId == reviewId).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(string authorId)
        {
            return await _comments.Find(c => c.AuthorId == authorId).ToListAsync();
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            await _comments.InsertOneAsync(comment);
        }

        public async Task<bool> UpdateCommentStatusAsync(string id, CommentStatus status)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
            var update = Builders<Comment>.Update.Set(c => c.Status, status);
            var result = await _comments.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteCommentAsync(string id)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
            var result = await _comments.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _comments.Find(FilterDefinition<Comment>.Empty).ToListAsync(); 
        }
    }
}

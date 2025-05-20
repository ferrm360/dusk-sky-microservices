using MongoDB.Driver;
using CommentService.Api.Models;
using CommentService.Api.Repositories.Interfaces;
using CommentService.Api.Enums;
using MongoDB.Bson;

namespace CommentService.Api.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentRepository(IMongoDatabase database)
        {
            _comments = database.GetCollection<Comment>("comments");
        }

        /// <summary>
        /// Obtiene todos los comentarios asociados a una reseña.
        /// </summary>
        /// <param name="reviewId">ID de la reseña.</param>
        /// <returns>Lista de comentarios.</returns>
        public async Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId)
        {
            return await _comments.Find(c => c.ReviewId == reviewId).ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los comentarios asociados a un autor.
        /// </summary>
        /// <param name="authorId">ID del autor.</param>
        /// <returns>Lista de comentarios.</returns>
        public async Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(string authorId)
        {
            return await _comments.Find(c => c.AuthorId == authorId).ToListAsync();
        }

        /// <summary>
        /// Crea un nuevo comentario.
        /// </summary>
        /// <param name="comment">Comentario a crear.</param>
        public async Task CreateCommentAsync(Comment comment)
        {
            await _comments.InsertOneAsync(comment);
        }

        /// <summary>
        /// Actualiza el estado de un comentario.
        /// </summary>
        /// <param name="id">ID del comentario.</param>
        /// <param name="status">Nuevo estado del comentario.</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario.</returns>
        public async Task<bool> UpdateCommentStatusAsync(string id, CommentStatus status)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, id);
            var update = Builders<Comment>.Update.Set(c => c.Status, status);
            var result = await _comments.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Elimina un comentario por su ID.
        /// </summary>
        /// <param name="id">ID del comentario a eliminar.</param>
        /// <returns>True si se eliminó correctamente, false en caso contrario.</returns>
        public async Task<bool> DeleteCommentAsync(string id)
        {
            try
            {
                var filter = Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(id));
                var result = await _comments.DeleteOneAsync(filter);
                return result.DeletedCount > 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los comentarios.
        /// </summary>
        /// <returns>Lista de todos los comentarios.</returns>
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _comments.Find(FilterDefinition<Comment>.Empty).ToListAsync();
        }

        /// <summary>
        /// Obtiene un comentario por su ID.
        /// </summary>
        /// <param name="id">ID del comentario.</param>
        /// <returns>El comentario encontrado o null si no existe.</returns>
        public async Task<Comment?> GetCommentByIdAsync(string id)
        {
            var filter = Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _comments.Find(filter).FirstOrDefaultAsync();
        }
    }
}
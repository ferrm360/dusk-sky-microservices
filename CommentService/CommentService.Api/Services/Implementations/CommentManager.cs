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

        /// <summary>
        /// Crea un nuevo comentario.
        /// </summary>
        /// <param name="comment">El comentario a crear.</param>
        /// <returns>El comentario creado.</returns>
        public async Task<Comment?> CreateCommentAsync(Comment comment)
        {
            await _repo.CreateCommentAsync(comment);
            return comment;
        }

        /// <summary>
        /// Obtiene todos los comentarios asociados a una reseña.
        /// </summary>
        /// <param name="reviewId">El ID de la reseña.</param>
        /// <returns>Una lista de comentarios.</returns>
        public Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId)
            => _repo.GetCommentsByReviewIdAsync(reviewId);

        /// <summary>
        /// Obtiene un comentario por su ID.
        /// </summary>
        /// <param name="id">El ID del comentario.</param>
        /// <returns>El comentario encontrado o null si no existe.</returns>
        public async Task<Comment?> GetCommentByIdAsync(string id)
        {
            return await _repo.GetCommentByIdAsync(id);
        }

        /// <summary>
        /// Obtiene todos los comentarios.
        /// </summary>
        /// <returns>Una lista de todos los comentarios.</returns>
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _repo.GetAllCommentsAsync();
        }

        /// <summary>
        /// Actualiza el estado de un comentario.
        /// </summary>
        /// <param name="id">El ID del comentario.</param>
        /// <param name="status">El nuevo estado del comentario.</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario.</returns>
        public async Task<bool> UpdateCommentStatusAsync(string id, CommentStatus status)
            => await _repo.UpdateCommentStatusAsync(id, status);

        /// <summary>
        /// Elimina un comentario por su ID.
        /// </summary>
        /// <param name="id">El ID del comentario a eliminar.</param>
        /// <returns>True si se eliminó correctamente, false en caso contrario.</returns>
        public async Task<bool> DeleteCommentAsync(string id)
            => await _repo.DeleteCommentAsync(id);
    }
}
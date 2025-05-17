using CommentService.Api.Models;
using CommentService.Api.Enums;

namespace CommentService.Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de comentarios.
    /// </summary>
    public interface ICommentManager
    {
        /// <summary>
        /// Crea un nuevo comentario.
        /// </summary>
        /// <param name="comment">Comentario a crear.</param>
        /// <returns>Comentario creado.</returns>
        Task<Comment?> CreateCommentAsync(Comment comment);

        /// <summary>
        /// Obtiene todos los comentarios asociados a una reseña.
        /// </summary>
        /// <param name="reviewId">ID de la reseña.</param>
        /// <returns>Lista de comentarios.</returns>
        Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId);

        /// <summary>
        /// Actualiza el estado de un comentario.
        /// </summary>
        /// <param name="id">ID del comentario.</param>
        /// <param name="status">Nuevo estado del comentario.</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario.</returns>
        Task<bool> UpdateCommentStatusAsync(string id, CommentStatus status);

        /// <summary>
        /// Elimina un comentario por su ID.
        /// </summary>
        /// <param name="id">ID del comentario a eliminar.</param>
        /// <returns>True si se eliminó correctamente, false en caso contrario.</returns>
        Task<bool> DeleteCommentAsync(string id);
    }
}
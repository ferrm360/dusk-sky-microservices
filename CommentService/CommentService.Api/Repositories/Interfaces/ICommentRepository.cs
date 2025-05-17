using CommentService.Api.Models;
using CommentService.Api.Enums;

namespace CommentService.Api.Repositories.Interfaces;

public interface ICommentRepository
{
    /// <summary>
    /// Obtiene un comentario por su ID.
    /// </summary>
    /// <param name="id">ID del comentario.</param>
    /// <returns>El comentario encontrado o null si no existe.</returns>
    Task<Comment?> GetCommentByIdAsync(string id);

    /// <summary>
    /// Obtiene todos los comentarios asociados a una reseña.
    /// </summary>
    /// <param name="reviewId">ID de la reseña.</param>
    /// <returns>Lista de comentarios.</returns>
    Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(string reviewId);

    /// <summary>
    /// Obtiene todos los comentarios asociados a un autor.
    /// </summary>
    /// <param name="authorId">ID del autor.</param>
    /// <returns>Lista de comentarios.</returns>
    Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(string authorId);

    /// <summary>
    /// Crea un nuevo comentario.
    /// </summary>
    /// <param name="comment">Comentario a crear.</param>
    Task CreateCommentAsync(Comment comment);

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

    /// <summary>
    /// Obtiene todos los comentarios.
    /// </summary>
    /// <returns>Lista de todos los comentarios.</returns>
    Task<IEnumerable<Comment>> GetAllCommentsAsync();
}
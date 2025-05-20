using CommentService.Api.Repositories.Interfaces;
using CommentService.Api.Models;
using CommentService.Api.Enums;

namespace CommentService.Api.Controllers;

public class CommentController
{
    private readonly ICommentRepository _commentRepository;

    public CommentController(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    /// <summary>
    /// Crea un nuevo comentario.
    /// </summary>
    /// <param name="comment">El comentario a crear.</param>
    /// <returns>El resultado de la operación.</returns>
    public async Task<IResult> CreateCommentAsync(Comment comment)
    {
        await _commentRepository.CreateCommentAsync(comment);
        return Results.Created($"/comments/{comment.Id}", comment);
    }

    /// <summary>
    /// Obtiene todos los comentarios.
    /// </summary>
    /// <returns>Una lista de todos los comentarios.</returns>
    public async Task<IResult> GetAllCommentsAsync()
    {
        var comments = await _commentRepository.GetAllCommentsAsync();
        return Results.Ok(comments);
    }

    /// <summary>
    /// Obtiene un comentario por su ID.
    /// </summary>
    /// <param name="id">El ID del comentario.</param>
    /// <returns>El comentario encontrado o un resultado de no encontrado.</returns>
    public async Task<IResult> GetCommentByIdAsync(string id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        return comment != null ? Results.Ok(comment) : Results.NotFound();
    }

    /// <summary>
    /// Obtiene todos los comentarios asociados a una reseña.
    /// </summary>
    /// <param name="reviewId">El ID de la reseña.</param>
    /// <returns>Una lista de comentarios asociados a la reseña.</returns>
    public async Task<IResult> GetCommentsByReviewIdAsync(string reviewId)
    {
        var comments = await _commentRepository.GetCommentsByReviewIdAsync(reviewId);
        return Results.Ok(comments);
    }

    /// <summary>
    /// Actualiza el estado de un comentario.
    /// </summary>
    /// <param name="id">El ID del comentario.</param>
    /// <param name="status">El nuevo estado del comentario.</param>
    /// <returns>El resultado de la operación.</returns>
    public async Task<IResult> UpdateCommentStatusAsync(string id, CommentStatus status)
    {
        var updated = await _commentRepository.UpdateCommentStatusAsync(id, status);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    /// <summary>
    /// Elimina un comentario por su ID.
    /// </summary>
    /// <param name="id">El ID del comentario a eliminar.</param>
    /// <returns>El resultado de la operación.</returns>
    public async Task<IResult> DeleteCommentAsync(string id)
    {
        var deleted = await _commentRepository.DeleteCommentAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
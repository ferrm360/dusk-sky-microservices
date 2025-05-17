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

    public async Task<IResult> CreateCommentAsync(Comment comment)
    {
        await _commentRepository.CreateCommentAsync(comment);
        return Results.Created($"/comments/{comment.Id}", comment);
    }

    public async Task<IResult> GetCommentsByReviewIdAsync(string reviewId)
    {
        var comments = await _commentRepository.GetCommentsByReviewIdAsync(reviewId);
        return Results.Ok(comments);
    }

    public async Task<IResult> UpdateCommentStatusAsync(string id, CommentStatus status)
    {
        await _commentRepository.UpdateCommentStatusAsync(id, status);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteCommentAsync(string id)
    {
        await _commentRepository.DeleteCommentAsync(id);
        return Results.NoContent();
    }
}

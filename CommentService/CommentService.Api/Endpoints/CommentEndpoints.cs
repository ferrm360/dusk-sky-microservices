using CommentService.Api.Models;
using Microsoft.AspNetCore.OpenApi;
using CommentService.Api.Services.Interfaces;

namespace CommentService.Api.Endpoints
{
    /// <summary>
    /// Provides extension methods to map comment-related endpoints to the web application.
    /// </summary>
    public static class CommentEndpoints
    {
        /// <summary>
        /// Maps the comment endpoints (GET, POST, DELETE) to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="WebApplication"/> instance.</param>
        public static void MapCommentEndpoints(this WebApplication app)
        {
            /// <summary>
            /// GET /comments
            /// Retrieves all comments (optionally by review ID).
            /// </summary>
            app.MapGet("/comments", async (ICommentManager commentService) =>
            {
                var comments = await commentService.GetCommentsByReviewIdAsync(string.Empty);
                return Results.Ok(comments);
            })
            .WithName("GetAllComments")
            .Produces<IEnumerable<Comment>>(StatusCodes.Status200OK)
            .WithOpenApi();

            /// <summary>
            /// POST /comments
            /// Creates a new comment.
            /// </summary>
            app.MapPost("/comments", async (ICommentManager commentService, Comment comment) =>
            {
                if (comment == null)
                {
                    return Results.BadRequest("Invalid comment.");
                }

                var createdComment = await commentService.CreateCommentAsync(comment);
                if (createdComment == null)
                {
                    return Results.StatusCode(500); 
                }

                return Results.Created($"/comments/{createdComment.Id}", createdComment);
            })
            .WithName("CreateComment")
            .Produces<Comment>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

            /// <summary>
            /// DELETE /comments/{id}
            /// Deletes a comment by its ID.
            /// </summary>
            app.MapDelete("/comments/{id}", async (ICommentManager commentService, string id) =>
            {
                var result = await commentService.DeleteCommentAsync(id);
                return result ? Results.Ok() : Results.NotFound();
            })
            .WithName("DeleteComment")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}
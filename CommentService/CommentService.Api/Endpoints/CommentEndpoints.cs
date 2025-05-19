using CommentService.Api.Models;
using Microsoft.AspNetCore.OpenApi;
using CommentService.Api.Services.Interfaces;
using MongoDB.Bson;
using CommentService.Api.Enums;

namespace CommentService.Api.Endpoints
{
    /// <summary>
    /// Provides extension methods to map comment-related endpoints to the web application.
    /// </summary>
    public static class CommentEndpoints
    {
        /// <summary>
        /// Maps the comment endpoints (GET, POST, DELETE, PUT) to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="WebApplication"/> instance.</param>
        public static void MapCommentEndpoints(this WebApplication app)
        {
            /// <summary>
            /// GET /comments
            /// Retrieves all comments.
            /// </summary>
            app.MapGet("/comments", async (ICommentManager commentService) =>
            {
                var comments = await commentService.GetAllCommentsAsync();
                return Results.Ok(comments);
            })
            .WithName("GetAllComments")
            .Produces<IEnumerable<Comment>>(StatusCodes.Status200OK)
            .WithOpenApi();

            /// <summary>
            /// GET /comments/{id}
            /// Retrieves a comment by its ID.
            /// </summary>
            app.MapGet("/comments/{id}", async (ICommentManager commentService, string id) =>
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Results.BadRequest("Invalid ID format.");
                }

                var comment = await commentService.GetCommentByIdAsync(id);
                return comment != null ? Results.Ok(comment) : Results.NotFound();
            })
            .WithName("GetCommentById")
            .Produces<Comment>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
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
            /// PUT /comments/{id}
            /// Updates the status of a comment by its ID.
            /// </summary>
            app.MapPut("/comments/{id}", async (ICommentManager commentService, string id, CommentStatus status) =>
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Results.BadRequest("Invalid ID format.");
                }

                var updated = await commentService.UpdateCommentStatusAsync(id, status);
                return updated ? Results.Ok() : Results.NotFound();
            })
            .WithName("UpdateCommentStatus")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            /// <summary>
            /// DELETE /comments/{id}
            /// Deletes a comment by its ID.
            /// </summary>
            app.MapDelete("/comments/{id}", async (ICommentManager commentService, string id) =>
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return Results.BadRequest("Invalid ID format.");
                }

                var result = await commentService.DeleteCommentAsync(id);
                return result ? Results.Ok() : Results.NotFound();
            })
            .WithName("DeleteComment")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}
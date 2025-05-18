using GameListService.Api.Models;
using GameListService.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;

namespace GameListService.Api.Endpoints
{
    public static class GameListItemEndpoints
    {
        public static void MapGameListItemEndpoints(this WebApplication app)
        {
            app.MapGet("/lists/{listId}/items", async (IGameListManager manager, string listId) =>
            {
                var items = await manager.GetItemsByListIdAsync(listId);
                return Results.Ok(items);
            })
            .WithName("GetItemsByListId")
            .Produces<IEnumerable<GameListItem>>(StatusCodes.Status200OK)
            .WithOpenApi();

            app.MapPost("/lists/{listId}/items", async (IGameListManager manager, string listId, GameListItem item) =>
            {
                item.ListId = listId;
                await manager.AddItemToListAsync(item);
                return Results.Created($"/lists/{listId}/items/{item.Id}", item);
            })
            .WithName("AddItemToList")
            .Produces<GameListItem>(StatusCodes.Status201Created)
            .WithOpenApi();

            app.MapPut("/lists/{listId}/items/{itemId}", async (IGameListManager manager, string listId, string itemId, GameListItem item) =>
            {
                item.Id = itemId;
                item.ListId = listId;
                var result = await manager.UpdateItemAsync(item);
                return result ? Results.Ok() : Results.NotFound();
            })
            .WithName("UpdateItem")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapDelete("/lists/{listId}/items/{itemId}", async (IGameListManager manager, string itemId) =>
            {
                var result = await manager.DeleteItemAsync(itemId);
                return result ? Results.Ok() : Results.NotFound();
            })
            .WithName("DeleteItem")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}

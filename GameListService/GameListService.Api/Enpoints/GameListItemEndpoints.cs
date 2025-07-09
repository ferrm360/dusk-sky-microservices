using GameListService.Api.Controllers;
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
                var controller = new GameListItemController(manager);
                return await controller.GetItemsByListIdAsync(listId);
            })
            .WithName("GetGameListItems")
            .Produces<IEnumerable<GameListItem>>(StatusCodes.Status200OK)
            .WithOpenApi();

            app.MapPost("/lists/{listId}/items", async (IGameListManager manager, string listId, GameListItem item) =>
            {
                if (item.ListId != listId)
                    return Results.BadRequest("ListId in URL and body do not match.");

                var controller = new GameListItemController(manager);
                return await controller.AddItemAsync(item);
            })
            .WithName("AddGameListItem")
            .Produces<GameListItem>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

            app.MapPut("/lists/{listId}/items/{itemId}", async (IGameListManager manager, string listId, string itemId, GameListItem item) =>
            {
                if (item.Id != itemId || item.ListId != listId)
                    return Results.BadRequest("ItemId or ListId in URL and body do not match.");

                var controller = new GameListItemController(manager);
                return await controller.UpdateItemAsync(item);
            })
            .WithName("UpdateGameListItem")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapDelete("/lists/{listId}/items/{itemId}", async (IGameListManager manager, string listId, string itemId) =>
            {
                var controller = new GameListItemController(manager);
                return await controller.DeleteItemAsync(itemId);
            })
            .WithName("DeleteGameListItem")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}

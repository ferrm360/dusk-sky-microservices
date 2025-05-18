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

            app.MapPost("/lists/items", async (IGameListManager manager, GameListItem item) =>
            {
                var controller = new GameListItemController(manager);
                return await controller.AddItemAsync(item);
            })
            .WithName("AddGameListItem")
            .Produces<GameListItem>(StatusCodes.Status201Created)
            .WithOpenApi();

            app.MapPut("/lists/items", async (IGameListManager manager, GameListItem item) =>
            {
                var controller = new GameListItemController(manager);
                return await controller.UpdateItemAsync(item);
            })
            .WithName("UpdateGameListItem")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapDelete("/lists/items/{itemId}", async (IGameListManager manager, string itemId) =>
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

using GameListService.Api.Controllers;
using GameListService.Api.Models;
using GameListService.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;

namespace GameListService.Api.Endpoints
{
    public static class GameListEndpoints
    {
        public static void MapGameListEndpoints(this WebApplication app)
        {
            app.MapGet("/lists/user/{userId}", async (IGameListManager manager, string userId) =>
            {
                var controller = new GameListController(manager);
                return await controller.GetListsByUserAsync(userId);
            })
            .WithName("GetUserGameLists")
            .Produces<IEnumerable<GameList>>(StatusCodes.Status200OK)
            .WithOpenApi();

            app.MapGet("/lists/{id}", async (IGameListManager manager, string id) =>
            {
                var controller = new GameListController(manager);
                return await controller.GetListByIdAsync(id);
            })
            .WithName("GetGameListById")
            .Produces<GameList>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapPost("/lists", async (IGameListManager manager, GameList list) =>
            {
                var controller = new GameListController(manager);
                return await controller.CreateListAsync(list);
            })
            .WithName("CreateGameList")
            .Produces<GameList>(StatusCodes.Status201Created)
            .WithOpenApi();

            app.MapPut("/lists/{id}", async (IGameListManager manager, string id, GameList list) =>
            {
                var controller = new GameListController(manager);
                return await controller.UpdateListAsync(id, list);
            })
            .WithName("UpdateGameList")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapDelete("/lists/{id}", async (IGameListManager manager, string id) =>
            {
                var controller = new GameListController(manager);
                return await controller.DeleteListAsync(id);
            })
            .WithName("DeleteGameList")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapGet("/lists/recent", async (IGameListManager manager) =>
            {
                var controller = new GameListController(manager);
                return await controller.GetMostRecentListsAsync();
            })
            .WithName("GetMostRecentLists")
            .Produces<IEnumerable<GameList>>(StatusCodes.Status200OK)
            .WithOpenApi();

            app.MapGet("/lists/popular", async (IGameListManager manager) =>
            {
                var controller = new GameListController(manager);
                return await controller.GetMostLikedListsAsync();
            })
            .WithName("GetMostLikedLists")
            .Produces<IEnumerable<GameList>>(StatusCodes.Status200OK)
            .WithOpenApi();

            app.MapPut("/lists/like/{id}", async (IGameListManager manager, string id) =>
            {
                var controller = new GameListController(manager);
                return await controller.LikeListAsync(id);
            })
            .WithName("LikeGameList")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();


            app.MapPut("/lists/unlike/{id}", async (IGameListManager manager, string id) =>
            {
                var controller = new GameListController(manager);
                return await controller.UnlikeListAsync(id);
            })
            .WithName("UnlikeGameList")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        }
    }
}

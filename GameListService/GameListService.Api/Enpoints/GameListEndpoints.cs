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
            app.MapGet("/gamelists/user/{userId}", async (IGameListManager manager, string userId) =>
            {
                var lists = await manager.GetUserListsAsync(userId);
                return Results.Ok(lists);
            })
            .WithName("GetUserGameLists")
            .Produces<IEnumerable<GameList>>(StatusCodes.Status200OK)
            .WithOpenApi();

            app.MapGet("/gamelists/{id}", async (IGameListManager manager, string id) =>
            {
                var list = await manager.GetListByIdAsync(id);
                return list is not null ? Results.Ok(list) : Results.NotFound();
            })
            .WithName("GetGameListById")
            .Produces<GameList>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapPost("/gamelists", async (IGameListManager manager, GameList list) =>
            {
                await manager.CreateListAsync(list);
                return Results.Created($"/gamelists/{list.Id}", list);
            })
            .WithName("CreateGameList")
            .Produces<GameList>(StatusCodes.Status201Created)
            .WithOpenApi();

            app.MapPut("/gamelists/{id}", async (IGameListManager manager, string id, GameList list) =>
            {
                if (id != list.Id)
                    return Results.BadRequest("Mismatched ID.");

                var success = await manager.UpdateListAsync(list);
                return success ? Results.Ok() : Results.NotFound();
            })
            .WithName("UpdateGameList")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            app.MapDelete("/gamelists/{id}", async (IGameListManager manager, string id) =>
            {
                var deleted = await manager.DeleteListAsync(id);
                return deleted ? Results.Ok() : Results.NotFound();
            })
            .WithName("DeleteGameList")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}

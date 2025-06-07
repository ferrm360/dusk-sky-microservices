using GameListService.Api.Models;
using GameListService.Api.Services.Interfaces;

namespace GameListService.Api.Controllers;

public class GameListController
{
    private readonly IGameListManager _manager;

    public GameListController(IGameListManager manager)
    {
        _manager = manager;
    }

    public async Task<IResult> GetListsByUserAsync(string userId)
    {
        var lists = await _manager.GetUserListsAsync(userId);
        return Results.Ok(lists);
    }

    public async Task<IResult> GetListByIdAsync(string id)
    {
        var list = await _manager.GetListByIdAsync(id);
        return list is not null ? Results.Ok(list) : Results.NotFound();
    }

    public async Task<IResult> CreateListAsync(GameList list)
    {
        await _manager.CreateListAsync(list);
        return Results.Created($"/lists/{list.Id}", list);
    }

    public async Task<IResult> UpdateListAsync(string id, GameList list)
    {
        if (id != list.Id)
            return Results.BadRequest("Mismatched ID.");

        var updated = await _manager.UpdateListAsync(list);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    public async Task<IResult> DeleteListAsync(string id)
    {
        var deleted = await _manager.DeleteListAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

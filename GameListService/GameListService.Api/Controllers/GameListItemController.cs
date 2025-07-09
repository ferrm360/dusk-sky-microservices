using GameListService.Api.Models;
using GameListService.Api.Services.Interfaces;

namespace GameListService.Api.Controllers;

public class GameListItemController
{
    private readonly IGameListManager _manager;

    public GameListItemController(IGameListManager manager)
    {
        _manager = manager;
    }

    public async Task<IResult> GetItemsByListIdAsync(string listId)
    {
        var items = await _manager.GetItemsByListIdAsync(listId);
        return Results.Ok(items);
    }

    public async Task<IResult> AddItemAsync(GameListItem item)
    {
        await _manager.AddItemToListAsync(item);
        return Results.Created($"/lists/{item.ListId}/items/{item.Id}", item);
    }

    public async Task<IResult> UpdateItemAsync(GameListItem item)
    {
        var updated = await _manager.UpdateItemAsync(item);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    public async Task<IResult> DeleteItemAsync(string itemId)
    {
        var deleted = await _manager.DeleteItemAsync(itemId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

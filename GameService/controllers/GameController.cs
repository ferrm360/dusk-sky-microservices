// Controllers/GameController.cs
using GameService.DTOs;
using GameService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("previews")]
    public async Task<ActionResult<List<GamePreviewDTO>>> GetPreviews()
    {
        var previews = await _gameService.GetAllGamePreviewsAsync();
        return Ok(previews);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var details = await _gameService.GetFullGameDetailsAsync(id);
        return details == null ? NotFound() : Ok(details);
    }

    [HttpPost("import/{steamAppId}")]
    public async Task<IActionResult> Import(int steamAppId)
    {
        var existing = await _gameService.GetBySteamAppIdAsync(steamAppId);
        if (existing != null)
        {
            return Conflict(new
            {
                message = "El juego ya fue importado previamente.",
                gameId = existing.Id
            });
        }

        var newId = await _gameService.ImportGameFromSteamAsync(steamAppId);
        if (newId == null)
        {
            return BadRequest("No se pudo importar el juego desde Steam.");
        }

        return CreatedAtAction(nameof(GetById), new { id = newId }, new
        {
            message = "Juego importado exitosamente.",
            gameId = newId
        });
    }

    

}

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/trackings")]
public class GameTrackingController : ControllerBase
{
    private readonly IGameTrackingService _service;

    public GameTrackingController(IGameTrackingService service)
    {
        _service = service;
    }

    // GET api/trackings/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var result = await _service.GetTrackingsByUserAsync(userId);
        return Ok(result);
    }

    // GET api/trackings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // POST api/trackings
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GameTrackingDTO dto)
    {
        var created = await _service.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/trackings/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] GameTrackingDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    // DELETE api/trackings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // GET api/trackings/user/{userId}/status/{status}
    [HttpGet("user/{userId}/status/{status}")]
    public async Task<IActionResult> GetGameIdsByStatus(string userId, string status)
    {
        var gameIds = await _service.GetGameIdsByStatusAsync(userId, status);
        return Ok(gameIds);
    }

    // GET api/trackings/user/{userId}/liked
    [HttpGet("user/{userId}/liked")]
    public async Task<IActionResult> GetLikedGameIds(string userId)
    {
        var gameIds = await _service.GetLikedGameIdsAsync(userId);
        return Ok(gameIds);
    }

    [HttpPost("lookup")]
    public async Task<IActionResult> GetByUserAndGameBody([FromBody] GameTrackingQueryKey key)
    {
        var tracking = await _service.GetByUserAndGameAsync(key.UserId, key.GameId);
        return tracking is null ? NotFound() : Ok(tracking);
    }


}
using Microsoft.AspNetCore.Http;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;

namespace ModerationService.Api.Controllers;

public class SanctionController
{
    private readonly ISanctionRepository _sanctionRepository;

    public SanctionController(ISanctionRepository sanctionRepository)
    {
        _sanctionRepository = sanctionRepository;
    }

    public async Task<IResult> GetAllSanctionsAsync()
    {
        var sanctions = await _sanctionRepository.GetAllAsync();
        return Results.Ok(sanctions);
    }

    public async Task<IResult> GetSanctionByIdAsync(string id)
    {
        var sanction = await _sanctionRepository.GetByIdAsync(id);
        return sanction is not null ? Results.Ok(sanction) : Results.NotFound();
    }

    public async Task<IResult> CreateSanctionAsync(Sanction sanction)
    {
        await _sanctionRepository.CreateAsync(sanction);
        return Results.Created($"/sanctions/{sanction.Id}", sanction);
    }

    public async Task<IResult> UpdateSanctionAsync(string id, Sanction sanction)
    {
        if (id != sanction.Id)
            return Results.BadRequest("Mismatched ID.");

        var updated = await _sanctionRepository.UpdateAsync(sanction);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    public async Task<IResult> DeleteSanctionAsync(string id)
    {
        var deleted = await _sanctionRepository.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

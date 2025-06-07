using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;

namespace ModerationService.Api.Endpoints
{
    public static class SanctionEndpoints
    {
        public static void MapSanctionEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/sanctions", async (ISanctionRepository repo) =>
            {
                var sanctions = await repo.GetAllAsync();
                return Results.Ok(sanctions);
            })
            .WithName("GetAllSanctions")
            .Produces<IEnumerable<Sanction>>(StatusCodes.Status200OK)
            .WithOpenApi();

            group.MapGet("/sanctions/{id}", async (ISanctionRepository repo, string id) =>
            {
                var sanction = await repo.GetByIdAsync(id);
                return sanction != null ? Results.Ok(sanction) : Results.NotFound();
            })
            .WithName("GetSanctionById")
            .Produces<Sanction>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            group.MapPost("/sanctions", async (ISanctionRepository repo, Sanction sanction) =>
            {
                await repo.CreateAsync(sanction);
                return Results.Created($"/moderation/sanctions/{sanction.Id}", sanction);
            })
            .WithName("CreateSanction")
            .Produces<Sanction>(StatusCodes.Status201Created)
            .WithOpenApi();

            group.MapPut("/sanctions/{id}", async (ISanctionRepository repo, string id, Sanction sanction) =>
            {
                if (id != sanction.Id)
                    return Results.BadRequest("Mismatched ID.");

                var updated = await repo.UpdateAsync(sanction);
                return updated ? Results.NoContent() : Results.NotFound();
            })
            .WithName("UpdateSanction")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            group.MapDelete("/sanctions/{id}", async (ISanctionRepository repo, string id) =>
            {
                var deleted = await repo.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteSanction")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using ModerationService.Api.Controllers;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;

namespace ModerationService.Api.Endpoints;

public static class SanctionEndpoints
{
    public static void MapSanctionEndpoints(this WebApplication app)
    {
        app.MapGet("/sanctions", async (ISanctionRepository repo) =>
        {
            var controller = new SanctionController(repo);
            return await controller.GetAllSanctionsAsync();
        })
        .WithName("GetAllSanctions")
        .Produces<IEnumerable<Sanction>>(StatusCodes.Status200OK)
        .WithOpenApi();

        app.MapGet("/sanctions/{id}", async (ISanctionRepository repo, string id) =>
        {
            var controller = new SanctionController(repo);
            return await controller.GetSanctionByIdAsync(id);
        })
        .WithName("GetSanctionById")
        .Produces<Sanction>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        app.MapPost("/sanctions", async (ISanctionRepository repo, Sanction sanction) =>
        {
            var controller = new SanctionController(repo);
            return await controller.CreateSanctionAsync(sanction);
        })
        .WithName("CreateSanction")
        .Produces<Sanction>(StatusCodes.Status201Created)
        .WithOpenApi();

        app.MapPut("/sanctions/{id}", async (ISanctionRepository repo, string id, Sanction sanction) =>
        {
            var controller = new SanctionController(repo);
            return await controller.UpdateSanctionAsync(id, sanction);
        })
        .WithName("UpdateSanction")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        app.MapDelete("/sanctions/{id}", async (ISanctionRepository repo, string id) =>
        {
            var controller = new SanctionController(repo);
            return await controller.DeleteSanctionAsync(id);
        })
        .WithName("DeleteSanction")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}

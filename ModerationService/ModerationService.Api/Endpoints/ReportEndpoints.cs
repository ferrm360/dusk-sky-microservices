using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;

namespace ModerationService.Api.Endpoints
{
    public static class ReportEndpoints
    {
        public static void MapReportEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/reports", async (IReportRepository repo) =>
            {
                var reports = await repo.GetAllAsync();
                return Results.Ok(reports);
            })
            .WithName("GetAllReports")
            .Produces<IEnumerable<Report>>(StatusCodes.Status200OK)
            .WithOpenApi();

            group.MapGet("/reports/{id}", async (IReportRepository repo, string id) =>
            {
                var report = await repo.GetByIdAsync(id);
                return report is not null ? Results.Ok(report) : Results.NotFound();
            })
            .WithName("GetReportById")
            .Produces<Report>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            group.MapPost("/reports", async (IReportRepository repo, Report report) =>
            {
                await repo.CreateAsync(report);
                return Results.Created($"/moderation/reports/{report.Id}", report);
            })
            .WithName("CreateReport")
            .Produces<Report>(StatusCodes.Status201Created)
            .WithOpenApi();

            group.MapPut("/reports/{id}", async (IReportRepository repo, string id, Report report) =>
            {
                if (id != report.Id)
                    return Results.BadRequest("Mismatched ID");

                var updated = await repo.UpdateAsync(report);
                return updated ? Results.NoContent() : Results.NotFound();
            })
            .WithName("UpdateReport")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

            group.MapDelete("/reports/{id}", async (IReportRepository repo, string id) =>
            {
                var deleted = await repo.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteReport")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        }
    }
}

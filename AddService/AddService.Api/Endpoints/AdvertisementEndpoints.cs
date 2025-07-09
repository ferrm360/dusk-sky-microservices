using AddService.Api.Models;
using AddService.Api.Services;

namespace AddService.Api.Endpoints;

public static class AdvertisementEndpoints
{
    public static void MapAdvertisementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ads").WithTags("Advertisements");

        group.MapGet("/", async (IAdvertisementService service) =>
        {
            var ads = await service.GetActiveAdsAsync();
            return Results.Ok(ads);
        });

        group.MapGet("/random", async (IAdvertisementService service) =>
        {
            var ad = await service.GetRandomAdAsync();
            return ad is not null ? Results.Ok(ad) : Results.NoContent();
        });

        group.MapGet("/{id}", async (string id, IAdvertisementService service) =>
        {
            var ad = await service.GetByIdAsync(id);
            return ad is not null ? Results.Ok(ad) : Results.NotFound();
        });

        group.MapPost("/", async (Advertisement ad, IAdvertisementService service) =>
        {
            await service.CreateAsync(ad);
            return Results.Created($"/ads/{ad.Id}", ad);
        });

        group.MapPut("/{id}", async (string id, Advertisement ad, IAdvertisementService service) =>
        {
            ad.Id = id;
            await service.UpdateAsync(ad);
            return Results.Ok(ad);
        });

        group.MapDelete("/{id}", async (string id, IAdvertisementService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}

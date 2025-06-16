using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore; // Para DbUpdateException
using Npgsql; // Para PostgresException
using ModerationService.Api.Models.Enums; // Para SanctionType

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
                if (string.IsNullOrWhiteSpace(sanction.Reason))
                {
                    return Results.BadRequest("La razón de la Razon es obligatoria.");
                }

                if (sanction.Type == SanctionType.suspension) // Accediendo correctamente a la enumeración
                {
                    if (!sanction.EndDate.HasValue)
                    {
                        return Results.BadRequest("La fecha de fin es obligatoria para una suspensión.");
                    }
                    if (sanction.EndDate.Value <= sanction.StartDate)
                    {
                        return Results.BadRequest("La fecha de fin debe ser posterior a la fecha de inicio para una suspensión.");
                    }
                }
                else if (sanction.Type == SanctionType.ban && sanction.EndDate.HasValue) // Accediendo correctamente a la enumeración
                {
                    return Results.BadRequest("Una sanción de tipo 'Ban' no debe tener una fecha de fin.");
                }

                try
                {
                    await repo.CreateAsync(sanction);
                    return Results.Created($"/moderation/sanctions/{sanction.Id}", sanction);
                }
                catch (DbUpdateException ex) // Captura excepciones de Entity Framework Core al actualizar/insertar
                {
                    var pgException = ex.InnerException as Npgsql.PostgresException;

                    if (pgException != null && pgException.SqlState == "23505" && pgException.ConstraintName == "IX_Sanction_report_id")
                    {
                        Console.WriteLine($"Conflicto de duplicidad al crear sanción: {pgException.Message}");
                        return Results.Conflict($"No se pudo aplicar la sanción. Ya existe una sanción asociada al reporte con ID '{sanction.ReportId}'.");
                    }
                    else
                    {
                        Console.Error.WriteLine($"Error al crear sanción (DbUpdateException): {ex.Message}");
                        return Results.Problem(
                            detail: "Ocurrió un error inesperado al guardar la sanción.",
                            statusCode: StatusCodes.Status500InternalServerError,
                            title: "Error Interno del Servidor"
                        );
                    }
                }
                catch (Exception ex) // Captura cualquier otra excepción inesperada
                {
                    Console.Error.WriteLine($"Error general al crear sanción: {ex.Message}");
                    return Results.Problem(
                        detail: "Ocurrió un error interno al procesar la sanción.",
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Error Interno del Servidor"
                    );
                }
            })
            .WithName("CreateSanction")
            .Produces<Sanction>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest) // Para validaciones de campos
            .Produces(StatusCodes.Status409Conflict)   // Para el caso específico de duplicidad en IX_Sanction_report_id
            .Produces(StatusCodes.Status500InternalServerError) // Para errores inesperados
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

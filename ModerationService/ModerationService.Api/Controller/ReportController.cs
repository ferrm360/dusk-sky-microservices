using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;

namespace ModerationService.Api.Controllers;

public class ReportController
{
    private readonly IReportRepository _repository;

    public ReportController(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> GetAllAsync()
    {
        var reports = await _repository.GetAllAsync();
        return Results.Ok(reports);
    }

    public async Task<IResult> GetByIdAsync(string id)
    {
        var report = await _repository.GetByIdAsync(id);
        return report is not null ? Results.Ok(report) : Results.NotFound();
    }

    public async Task<IResult> CreateAsync(Report report)
    {
        await _repository.CreateAsync(report);
        return Results.Created($"/reports/{report.Id}", report);
    }

    public async Task<IResult> UpdateAsync(string id, Report report)
    {
        if (id != report.Id)
            return Results.BadRequest("Mismatched ID");

        var updated = await _repository.UpdateAsync(report);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    public async Task<IResult> DeleteAsync(string id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

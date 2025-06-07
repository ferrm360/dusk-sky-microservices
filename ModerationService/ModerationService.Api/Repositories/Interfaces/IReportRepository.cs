using ModerationService.Api.Models;

namespace ModerationService.Api.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(string id);
        Task CreateAsync(Report report);
        Task<bool> UpdateStatusAsync(string id, string status);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(Report report);  
    }
}

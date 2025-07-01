using ModerationService.Api.Models;

namespace ModerationService.Api.Repositories.Interfaces
{
    public interface ISanctionRepository
    {
        Task<IEnumerable<Sanction>> GetAllAsync();
        Task<Sanction?> GetByIdAsync(string id);
        Task CreateAsync(Sanction sanction);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(Sanction sanction);
    }
}

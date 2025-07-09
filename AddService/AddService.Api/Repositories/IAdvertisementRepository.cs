using AddService.Api.Models;

namespace AddService.Api.Repositories;

public interface IAdvertisementRepository
{
    Task<List<Advertisement>> GetActiveAsync();
    Task<Advertisement?> GetByIdAsync(string id);
    Task CreateAsync(Advertisement ad);
    Task UpdateAsync(Advertisement ad);
    Task DeleteAsync(string id);
}

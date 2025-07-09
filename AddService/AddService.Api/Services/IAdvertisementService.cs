using AddService.Api.Models;

namespace AddService.Api.Services;

public interface IAdvertisementService
{
    Task<List<Advertisement>> GetActiveAdsAsync();
    Task<Advertisement?> GetByIdAsync(string id);
    Task CreateAsync(Advertisement ad);
    Task UpdateAsync(Advertisement ad);
    Task DeleteAsync(string id);
    Task<Advertisement?> GetRandomAdAsync();
}

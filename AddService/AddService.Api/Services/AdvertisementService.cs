using AddService.Api.Models;
using AddService.Api.Repositories;

namespace AddService.Api.Services;

public class AdvertisementService : IAdvertisementService
{
    private readonly IAdvertisementRepository _repository;
    private readonly Random _random = new();

    public AdvertisementService(IAdvertisementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Advertisement>> GetActiveAdsAsync()
    {
        return await _repository.GetActiveAsync();
    }

    public async Task<Advertisement?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Advertisement ad)
    {
        if (ad.Priority < 1 || ad.Priority > 10)
            throw new ArgumentException("La prioridad debe estar entre 1 y 10.");

        ad.CreatedAt = DateTime.UtcNow;
        await _repository.CreateAsync(ad);
    }

    public async Task UpdateAsync(Advertisement ad)
    {
        if (ad.Priority < 1 || ad.Priority > 10)
            throw new ArgumentException("La prioridad debe estar entre 1 y 10.");

        await _repository.UpdateAsync(ad);
    }

    public async Task DeleteAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<Advertisement?> GetRandomAdAsync()
    {
        var activeAds = await _repository.GetActiveAsync();
        if (!activeAds.Any())
            return null;

        var weightedList = activeAds.SelectMany(ad => Enumerable.Repeat(ad, ad.Priority)).ToList();
        var index = _random.Next(weightedList.Count);
        return weightedList[index];
    }
}

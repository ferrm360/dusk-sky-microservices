using AddService.Api.Infrastructure;
using AddService.Api.Models;
using MongoDB.Driver;

namespace AddService.Api.Repositories;

public class AdvertisementRepository : IAdvertisementRepository
{
    private readonly IMongoCollection<Advertisement> _collection;

    public AdvertisementRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Advertisement>("Advertisements");
    }

    public async Task<List<Advertisement>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _collection.Find(ad =>
            ad.Active && (ad.ValidUntil == null || ad.ValidUntil > now))
            .ToListAsync();
    }

    public async Task<Advertisement?> GetByIdAsync(string id)
    {
        return await _collection.Find(ad => ad.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Advertisement ad)
    {
        await _collection.InsertOneAsync(ad);
    }

    public async Task UpdateAsync(Advertisement ad)
    {
        await _collection.ReplaceOneAsync(a => a.Id == ad.Id, ad);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(a => a.Id == id);
    }
}

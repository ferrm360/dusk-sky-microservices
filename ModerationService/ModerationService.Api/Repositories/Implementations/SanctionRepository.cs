using Microsoft.EntityFrameworkCore;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;
using ModerationService.Api.Models.Enums;
using ModerationService.Api.Infrastructure;


namespace ModerationService.Api.Repositories.Implementations
{
    public class SanctionRepository : ISanctionRepository
    {
        private readonly ModerationDbContext _context;

        public SanctionRepository(ModerationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sanction>> GetAllAsync()
        {
            return await _context.Sanctions.Include(s => s.Report).ToListAsync();
        }

        public async Task<Sanction?> GetByIdAsync(string id)
        {
            return await _context.Sanctions.Include(s => s.Report).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task CreateAsync(Sanction sanction)
        {
            _context.Sanctions.Add(sanction);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Sanction sanction)
        {
            _context.Sanctions.Update(sanction);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var sanction = await GetByIdAsync(id);
            if (sanction == null) return false;

            _context.Sanctions.Remove(sanction);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ModerationService.Api.Models;
using ModerationService.Api.Repositories.Interfaces;

namespace ModerationService.Api.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly ModerationDbContext _context;

        public ReportRepository(ModerationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(string id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task CreateAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var report = await GetByIdAsync(id);
            if (report == null) return false;

            _context.Reports.Remove(report);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(string id, string status)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return false;

            if (!Enum.TryParse(status, ignoreCase: true, out ReportStatus parsedStatus))
                return false;

            report.Status = parsedStatus;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}

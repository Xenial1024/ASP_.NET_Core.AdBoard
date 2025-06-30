using AdBoard.Core;
using AdBoard.Core.Models.Domains;
using AdBoard.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AdBoard.Persistence.Repositories
{
    public class BrowseRepository (IApplicationDbContext context) : IBrowseRepository
    {
        readonly IApplicationDbContext _context = context;
        public async Task<List<Ad>> GetAdsByUserIdAsync(string userId) =>
            await _context.Ads
                .Include(ad => ad.Images)
                .Where(ad => ad.UserId == userId)
                .ToListAsync();

        public async Task<Ad> GetAdWithDetailsAsync(int id) =>
            await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Ad>> GetAdsExceptUserAsync(string excludeUserId) =>
            await _context.Ads
                .Include(ad => ad.Images)
                .Where(ad => ad.UserId != excludeUserId)
                .ToListAsync();
    }
}
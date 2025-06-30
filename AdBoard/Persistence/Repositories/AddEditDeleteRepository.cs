using AdBoard.Core;
using AdBoard.Core.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace AdBoard.Persistence.Repositories
{
    public class AddEditDeleteRepository (IApplicationDbContext context) : IAddEditDeleteRepository
    {
        readonly IApplicationDbContext _context = context;

        #region Ad Operations

        public async Task<Ad> GetAdByIdAsync(int id)
            => await _context.Ads
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Ad> AddAdAsync(Ad ad)
        {
            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();
            return ad;
        }

        public async Task<Ad> UpdateAdAsync(Ad ad)
        {
            _context.Ads.Update(ad);
            await _context.SaveChangesAsync();
            return ad;
        }

        public async Task<bool> DeleteAdAsync(int id)
        {
            var ad = await GetAdByIdAsync(id);
            if (ad == null) return false;

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdExistsAsync(int id)
            => await _context.Ads.AnyAsync(a => a.Id == id);

        public async Task<bool> IsAdOwnerAsync(int adId, string userId)
            => await _context.Ads.AnyAsync(a => a.Id == adId && a.UserId == userId);

        #endregion

        #region Image Operations

        public async Task<List<Image>> GetImagesByAdIdAsync(int adId)
            => await _context.Images
                .Where(img => img.AdId == adId)
                .ToListAsync();

        public async Task<List<Image>> GetImagesByIdsAsync(int adId, int[] imageIds)
            => await _context.Images
                .Where(img => img.AdId == adId && imageIds.Contains(img.Id))
                .ToListAsync();

        public async Task AddImagesAsync(List<Image> images)
        {
            _context.Images.AddRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveImagesAsync(List<Image> images)
        {
            _context.Images.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Image>> GetAllImagesByAdIdAsync(int adId)
            => await _context.Images
                .Where(img => img.AdId == adId)
                .ToListAsync();
        #endregion
    }
}
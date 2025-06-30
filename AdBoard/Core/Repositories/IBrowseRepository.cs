using AdBoard.Core.Models.Domains;

namespace AdBoard.Core.Repositories
{
    public interface IBrowseRepository
    {
        Task<List<Ad>> GetAdsByUserIdAsync(string userId);
        Task<Ad> GetAdWithDetailsAsync(int id);
        Task<List<Ad>> GetAdsExceptUserAsync(string excludeUserId);
    }
}
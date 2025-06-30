using AdBoard.Core.Models.Domains;

namespace AdBoard.Persistence.Services
{
    public interface IBrowseService
    {
        Task<List<Ad>> GetAdsForListingAsync(string userId, string currentUserId);
        Task<Ad> GetAdDetailsAsync(int id);
    }
}
using AdBoard.Core.Models.Domains;
using AdBoard.Core.Repositories;

namespace AdBoard.Persistence.Services
{
    public class BrowseService(IBrowseRepository browseRepository) : IBrowseService
    {
        private readonly IBrowseRepository _browseRepository = browseRepository;


        public async Task<List<Ad>> GetAdsForListingAsync(string userId, string currentUserId)
        {
            if (userId == "current")
                return await _browseRepository.GetAdsByUserIdAsync(currentUserId);

            return await _browseRepository.GetAdsExceptUserAsync(currentUserId);
        }

        public async Task<Ad> GetAdDetailsAsync(int id) =>
            await _browseRepository.GetAdWithDetailsAsync(id);
    }
}
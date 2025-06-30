using AdBoard.Core.Models.Domains;

namespace AdBoard.Persistence.Repositories
{
    public interface IAddEditDeleteRepository
    {
        // Operacje na Ad
        Task<Ad> GetAdByIdAsync(int id);
        Task<Ad> AddAdAsync(Ad ad);
        Task<Ad> UpdateAdAsync(Ad ad);
        Task<bool> DeleteAdAsync(int id);
        Task<bool> AdExistsAsync(int id);
        Task<bool> IsAdOwnerAsync(int adId, string userId);

        // Operacje na Image
        Task<List<Image>> GetImagesByAdIdAsync(int adId);
        Task<List<Image>> GetImagesByIdsAsync(int adId, int[] imageIds);
        Task AddImagesAsync(List<Image> images);
        Task RemoveImagesAsync(List<Image> images);
        Task<List<Image>> GetAllImagesByAdIdAsync(int adId);
    }
}
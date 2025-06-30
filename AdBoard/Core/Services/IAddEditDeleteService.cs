using AdBoard.Core.Models.Domains;

namespace AdBoard.Persistence.Services
{
    public interface IAddEditDeleteService
    {
        Task<Ad> AddAdAsync(Ad ad, IFormFileCollection images, string userId);
        Task<Ad> EditAdAsync(Ad model, IFormFileCollection newImages, int[] removeImages);
        Task<bool> DeleteAdAsync(int id, string userId);
        Task<Ad> GetAdByIdAsync(int id, string userId);
        Task AddImagesAsync(int adId, IFormFileCollection images);
        Task RemoveImagesAsync(int adId, int[] imageIds);
        Task RemoveAllImagesAsync(int adId);
    }
}
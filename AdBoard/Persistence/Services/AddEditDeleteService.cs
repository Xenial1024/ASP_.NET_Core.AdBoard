using AdBoard.Core.Models.Domains;
using AdBoard.Persistence.Repositories;

namespace AdBoard.Persistence.Services
{
    public class AddEditDeleteService(IAddEditDeleteRepository repository) : IAddEditDeleteService
    {
        readonly IAddEditDeleteRepository _repository = repository;
        static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<Ad> AddAdAsync(Ad ad, IFormFileCollection images, string userId)
        {
            ad.CreatedDate = DateOnly.FromDateTime(DateTime.Today);
            ad.UserId = userId;

            var createdAd = await _repository.AddAdAsync(ad);

            if (images != null && images.Count > 0)
            {
                var imageEntities = await ProcessUploadedImages(images, createdAd.Id);
                if (imageEntities.Count != 0)
                    await _repository.AddImagesAsync(imageEntities);
            }

            return createdAd;
        }

        public async Task<Ad> EditAdAsync(Ad ad, IFormFileCollection newImages, int[] removeImages)
        {
            Ad existingAd = await _repository.GetAdByIdAsync(ad.Id);
            if (existingAd == null) 
                return null;

            existingAd.Title = ad.Title;
            existingAd.Description = ad.Description;
            existingAd.Value = ad.Value;
            existingAd.Category = ad.Category;
            existingAd.Unit = ad.Unit;

            if (removeImages != null && removeImages.Length > 0)
            {
                List<Image> imagesToRemove = await _repository.GetImagesByIdsAsync(ad.Id, removeImages);
                if (imagesToRemove.Count > 0)
                {
                    foreach (Image img in imagesToRemove)
                    {
                        string fullPath = Path.Combine("wwwroot", img.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                        if (File.Exists(fullPath))
                            File.Delete(fullPath);
                    }

                    await _repository.RemoveImagesAsync(imagesToRemove);
                }
            }

            if (newImages != null && newImages.Count > 0)
            {
                List<Image> addedImages = new();

                foreach (IFormFile file in newImages)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    string folder = Path.Combine("wwwroot", "uploads", ad.UserId ?? "unknown");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string filePath = Path.Combine(folder, uniqueFileName);

                    using (FileStream fs = new(filePath, FileMode.Create))
                        await file.CopyToAsync(fs);

                    addedImages.Add(new Image
                    {
                        AdId = ad.Id,
                        FileName = uniqueFileName,
                        FilePath = "/" + Path.Combine("uploads", ad.UserId ?? "unknown", uniqueFileName).Replace("\\", "/")
                    });
                }

                await _repository.AddImagesAsync(addedImages);
            }

            await _repository.UpdateAdAsync(existingAd);
            return existingAd;
        }


        public async Task<bool> DeleteAdAsync(int id, string userId)
        {
            try
            {
                if (!await _repository.IsAdOwnerAsync(id, userId))
                    return false;

                await RemoveAllImagesAsync(id);

                return await _repository.DeleteAdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Błąd podczas usuwania ogłoszenia: {ex.Message}");
                throw;
            }
        }

        public async Task<Ad> GetAdByIdAsync(int id, string userId)
        {
            var ad = await _repository.GetAdByIdAsync(id);
            if (ad == null || ad.UserId != userId)
                return null;
            return ad;
        }

        public async Task AddImagesAsync(int adId, IFormFileCollection images)
        {
            var imageEntities = await ProcessUploadedImages(images, adId);
            if (imageEntities.Count != 0)
                await _repository.AddImagesAsync(imageEntities);
        }

        public async Task RemoveImagesAsync(int adId, int[] imageIds)
        {
            var imagesToRemove = await _repository.GetImagesByIdsAsync(adId, imageIds);

            foreach (var img in imagesToRemove)
                DeleteImageFile(img.FilePath);

            await _repository.RemoveImagesAsync(imagesToRemove);
        }

        public async Task RemoveAllImagesAsync(int adId)
        {
            var images = await _repository.GetAllImagesByAdIdAsync(adId);

            foreach (var img in images)
                DeleteImageFile(img.FilePath);

            await _repository.RemoveImagesAsync(images);
        }

        #region Private Helper Methods

        private static async Task<List<Image>> ProcessUploadedImages(IFormFileCollection images, int adId)
        {
            var imageEntities = new List<Image>();
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            foreach (IFormFile file in images)
            {
                if (file.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (FileStream stream = new(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imageEntities.Add(new Image
                    {
                        FileName = fileName,
                        FilePath = $"/uploads/{fileName}",
                        AdId = adId
                    });
                }
            }

            return imageEntities;
        }

        private static void DeleteImageFile(string filePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        #endregion
    }
}
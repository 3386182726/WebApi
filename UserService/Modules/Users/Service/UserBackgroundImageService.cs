using Common.Service;
using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Model;
using UserService.Modules.Users.Repository;

namespace UserService.Modules.Users.Service
{
    public class UserBackgroundImageService(
        IUserBackgroundImageRepository repository,
        IUploadService uploadService
    ) : IUserBackgroundImageService
    {
        public async Task<List<UserBackgroundImageResponse>> GetAllByUserIdAsync(string userId)
        {
            return await repository.GetResponsesByUserIdAsync(userId);
        }

        public async Task<UserBackgroundImageResponse> UploadAsync(IFormFile file, string userId)
        {
            var url = await uploadService.UploadAsync(file);

            var image = new UserBackgroundImage
            {
                UserId = userId,
                Name = file.FileName,
                Url = url,
                CreatedAt = DateTime.UtcNow,
            };

            repository.Create(image);
            await repository.SaveChangesAsync();

            return new UserBackgroundImageResponse
            {
                Id = image.Id,
                UserId = image.UserId,
                Name = image.Name,
                Url = image.Url,
                CreatedAt = image.CreatedAt,
            };
        }

        public async Task<UserBackgroundImage?> GetByIdAsync(string id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task DeleteAsync(UserBackgroundImage image)
        {
            repository.Remove(image);
            await repository.SaveChangesAsync();

            uploadService.Delete(image.Url);
        }
    }
}

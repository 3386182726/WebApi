using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Model;

namespace UserService.Modules.Users.Service
{
    public interface IUserBackgroundImageService
    {
        /// <summary>
        /// 上传当前用户的背景图
        /// </summary>
        Task<UserBackgroundImageResponse> UploadAsync(IFormFile file, string userId);

        /// <summary>
        /// 获取当前用户的背景图列表（按创建时间倒序）
        /// </summary>
        Task<List<UserBackgroundImageResponse>> GetAllByUserIdAsync(string userId);

        /// <summary>
        /// 按 Id 获取背景图
        /// </summary>
        Task<UserBackgroundImage?> GetByIdAsync(string id);

        /// <summary>
        /// 删除背景图记录及对应的物理文件
        /// </summary>
        Task DeleteAsync(UserBackgroundImage image);
    }
}

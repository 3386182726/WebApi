using Common.Abstractions;
using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Model;

namespace UserService.Modules.Users.Repository
{
    public interface IUserBackgroundImageRepository
        : IRepository<UserBackgroundImage, UserBackgroundImageResponse>
    {
        /// <summary>
        /// 查询指定用户的全部背景图（按创建时间倒序）
        /// </summary>
        Task<List<UserBackgroundImageResponse>> GetResponsesByUserIdAsync(string userId);
    }
}

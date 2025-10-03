using Common.Dto;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Repository
{
    public interface IUserRepository
    {
        public Task<PagedResult<UserResultDto>> GetUsersAsync(PagedRequest request);
    }
}

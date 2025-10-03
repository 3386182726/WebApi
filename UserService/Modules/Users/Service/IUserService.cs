using Common.Dto;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Service
{
    public interface IUserService
    {
        public Task<User?> ValidateUserAsync(LoginDto dto);
        public Task<PagedResult<UserResultDto>> GetUsersAsync(PagedRequest request);
    }
}

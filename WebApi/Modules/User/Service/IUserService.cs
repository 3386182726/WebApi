using WebApi.Dtos;
using WebApi.Modules.User.Dto;

namespace WebApi.Modules.User.Service
{
    public interface IUserService
    {
        public Task<User?> ValidateUserAsync(LoginDto dto);
        public Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request);
    }
}

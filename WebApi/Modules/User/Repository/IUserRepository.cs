using WebApi.Dtos;
using WebApi.Modules.User.Dto;

namespace WebApi.Modules.User.Repository
{
    public interface IUserRepository
    {
        public Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request);
    }
}

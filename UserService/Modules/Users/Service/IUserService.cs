using Common.Pagination;
using Contracts.Event.User;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Service
{
    public interface IUserService
    {
        public Task<User?> ValidateUserAsync(LoginRequest dto);
        public Task<PagedResult<UserResponse>> GetUsersAsync(PagedRequest request);
        public Task PublishUserUpsertEventAsync(UserUpsertEvent user);
        public Task<UserResponse?> GetCurrentUserAsync(string userId);
        public Task<List<UserResponse>> GetByOrganizationIdsAsync(IEnumerable<string> organizationIds);
    }
}

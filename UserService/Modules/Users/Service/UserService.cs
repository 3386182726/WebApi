using Common.Abstractions;
using Common.Pagination;
using Contracts.Event.User;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Service
{
    public class UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IRepository<User, UserResponse> userRepository,
        IPublishEndpoint publish
    ) : IUserService
    {
        public async Task<User?> ValidateUserAsync(LoginRequest dto)
        {
            var user = await userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return null;

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                lockoutOnFailure: false
            );
            return result.Succeeded ? user : null;
        }

        public async Task<PagedResult<UserResponse>> GetUsersAsync(PagedRequest request)
        {
            return await userRepository.GetListAsync(request);
        }

        public async Task PublishUserUpsertEventAsync(UserUpsertEvent user)
        {
            await publish.Publish(user);
        }

        public async Task<UserResponse?> GetCurrentUserAsync(string userId)
        {
            return await userRepository.GetResponseByIdAsync(userId);
        }

        public async Task<List<UserResponse>> GetByOrganizationIdsAsync(IEnumerable<string> organizationIds)
        {
            var ids = organizationIds.ToHashSet();
            if (ids.Count == 0) return [];
            return await userRepository.QueryResponsesAsync(u => ids.Contains(u.OrganizationId!));
        }
    }
}

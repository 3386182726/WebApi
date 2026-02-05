using Common.Pagination;
using Contracts.Event.User;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Repository;

namespace UserService.Modules.Users.Service
{
    public class UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserRepository userRepository,
        IPublishEndpoint publish
    ) : IUserService
    {
        public async Task<User?> ValidateUserAsync(LoginDto dto)
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

        public async Task<PagedResult<UserResultDto>> GetUsersAsync(PagedRequest request)
        {
            return await userRepository.GetUsersAsync(request);
        }

        public async Task PublishUserUpsertEventAsync(UserUpsertEvent user)
        {
            await publish.Publish(user);
        }
    }
}

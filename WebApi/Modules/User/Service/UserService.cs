using Microsoft.AspNetCore.Identity;
using WebApi.Dtos;
using WebApi.Modules.User.Dto;
using WebApi.Modules.User.Repository;

namespace WebApi.Modules.User.Service
{
    public class UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserRepository userRepository
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

        public async Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request)
        {
            return await userRepository.GetUsersAsync(request);
        }
    }
}

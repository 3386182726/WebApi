using Common.Abstractions;
using Common.Models;
using Common.Pagination;
using Contracts.Event.User;
using Flurl;
using Flurl.Http;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Service
{
    /// <summary>
    /// 用户业务：EF 查询委托给 UserRepository，跨服务图片组装自己做。
    /// </summary>
    public class UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IRepository<User, UserResponse> userRepository,
        IPublishEndpoint publish,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    ) : IUserService
    {
        public async Task<User?> ValidateUserAsync(LoginRequest dto)
        {
            var user = await userManager.FindByNameAsync(dto.Username);
            if (user == null) return null;

            var result = await signInManager.CheckPasswordSignInAsync(
                user, dto.Password, lockoutOnFailure: false);
            return result.Succeeded ? user : null;
        }

        public async Task<PagedResult<UserResponse>> GetUsersAsync(PagedRequest request)
        {
            var result = await userRepository.GetListAsync(request);
            if (result.Items.Count > 0)
                await FillImagesAsync(result.Items);
            return result;
        }

        public async Task<UserResponse?> GetCurrentUserAsync(string userId)
        {
            var user = await userRepository.GetResponseByIdAsync(userId);
            if (user != null)
                await FillImagesAsync([user]);
            return user;
        }

        public async Task<List<UserResponse>> GetByOrganizationIdsAsync(IEnumerable<string> organizationIds)
        {
            var ids = organizationIds.ToHashSet();
            if (ids.Count == 0) return [];
            var users = await userRepository.QueryResponsesAsync(u => ids.Contains(u.OrganizationId!));
            if (users.Count > 0)
                await FillImagesAsync(users);
            return users;
        }

        public async Task PublishUserUpsertEventAsync(UserUpsertEvent user)
        {
            await publish.Publish(user);
        }

        /// <summary>批量从 FileService 组装头像/照片到 UserResponse</summary>
        //如果是user用MassTransit 发给资源 事件获取头像，然后资源服务 收到后再给user服务发事件返回图片。
        //等user接收到返回的图片，用户列表已经返回给前端了,
        //还需要消费者单独处理把图片再写入到用户列表，所以这里直接用http异步请求。
        private async Task FillImagesAsync(IEnumerable<UserResponse> users)
        {
            var userIds = users.Select(u => u.Id).ToList();
            if (userIds.Count == 0) return;

            var baseUrl = configuration["FileService:BaseUrl"] ?? "http://fileservice:8080";
            using var httpClient = httpClientFactory.CreateClient();

            var query = new FileQueryRequest
            {
                OwnerType = AppFileConstants.OwnerTypeUser,
                OwnerIds = userIds,
                Category = AppFileConstants.CategoryImage,
            };

            var allFiles = await baseUrl
                .AppendPathSegments("api", "file", "query")
                .PostJsonAsync(query)
                .ReceiveJson<List<AppFileResponse>>() ?? [];

            var grouped = allFiles.GroupBy(f => f.OwnerId);

            foreach (var user in users)
            {
                var files = grouped.FirstOrDefault(g => g.Key == user.Id)?.ToList() ?? [];
                user.Avatars = files.Where(f => f.Type == AppFileConstants.TypeAvatar).ToList();
                user.Photos = files.Where(f => f.Type == AppFileConstants.TypePhoto).ToList();
            }
        }
    }
}

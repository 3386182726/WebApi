using Common.EFCore;
using Common.Models;
using Common.Pagination;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Users.Dto;
using System.Linq.Expressions;

namespace UserService.Modules.Users.Repository
{
    public class UserRepository(
        UserDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    ) : RepositoryBase<User, UserResponse, UserDbContext>(dbContext)
    {
        protected override HashSet<string> AllowedFields { get; } = ["UserName", "Name", "Email"];

        protected override Expression<Func<UserResponse, object>> DefaultOrderBy
            => x => x.UserName;

        protected override IQueryable<UserResponse> BuildResponseQuery()
        {
            return
                from u in DbContext.Users
                join d in DbContext.Organizations on u.OrganizationId equals d.Id into dj
                from d in dj.DefaultIfEmpty()
                join ur in DbContext.UserRoles on u.Id equals ur.UserId into urj
                from ur in urj.DefaultIfEmpty()
                join r in DbContext.Roles on ur.RoleId equals r.Id into rj
                from r in rj.DefaultIfEmpty()
                group new { r, d } by new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Name,
                    u.Introduction,
                    u.Birthday,
                    u.OrganizationId,
                } into g
                select new UserResponse
                {
                    Id = g.Key.Id,
                    UserName = g.Key.UserName,
                    Email = g.Key.Email,
                    Name = g.Key.Name,
                    Introduction = g.Key.Introduction,
                    Birthday = g.Key.Birthday,
                    OrganizationId = g.Key.OrganizationId,
                    OrganizationName = g.Select(x => x.d != null ? x.d.Name : null).FirstOrDefault(),
                    Roles = g.Where(x => x.r != null).Select(x => x.r.Name!).ToList(),
                };
        }

        public override async Task<UserResponse?> GetResponseByIdAsync(string id)
        {
            var user = await base.GetResponseByIdAsync(id);
            if (user != null)
                await FillAppFilesAsync([user]);
            return user;
        }

        public override async Task<PagedResult<UserResponse>> GetListAsync(PagedRequest request)
        {
            var result = await base.GetListAsync(request);
            if (result.Items.Count > 0)
                await FillAppFilesAsync(result.Items);
            return result;
        }

        public override async Task<List<UserResponse>> QueryResponsesAsync(Expression<Func<UserResponse, bool>> predicate)
        {
            var users = await base.QueryResponsesAsync(predicate);
            if (users.Count > 0)
                await FillAppFilesAsync(users);
            return users;
        }

        private async Task FillAppFilesAsync(IEnumerable<UserResponse> users)
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

            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/file/query", query);
            response.EnsureSuccessStatusCode();

            var allFiles = await response.Content.ReadFromJsonAsync<List<AppFileResponse>>() ?? [];
            var grouped = allFiles.GroupBy(f => f.OwnerId);

            foreach (var user in users)
            {
                var files = grouped.FirstOrDefault(g => g.Key == user.Id)?.ToList() ?? [];
                user.Avatars = files.Where(f => f.Type == AppFileConstants.TypeAvatar).ToList();
                user.Photos = files.Where(f => f.Type == AppFileConstants.TypePhoto).ToList();
            }
        }
    }

    public class FileQueryRequest
    {
        public required string OwnerType { get; set; }
        public required List<string> OwnerIds { get; set; } = [];
        public string? Category { get; set; }
    }
}

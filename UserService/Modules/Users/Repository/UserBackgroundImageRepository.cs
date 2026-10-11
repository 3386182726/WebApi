using System.Linq.Expressions;
using Common.EFCore;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Users.Dto;
using UserService.Modules.Users.Model;

namespace UserService.Modules.Users.Repository
{
    public class UserBackgroundImageRepository(UserDbContext dbContext)
        : RepositoryBase<UserBackgroundImage, UserBackgroundImageResponse, UserDbContext>(dbContext),
            IUserBackgroundImageRepository
    {
        protected override HashSet<string> AllowedFields { get; } = ["Name"];

        protected override Expression<Func<UserBackgroundImageResponse, object>> DefaultOrderBy
            => x => x.CreatedAt;

        protected override IQueryable<UserBackgroundImageResponse> BuildResponseQuery()
        {
            return DbContext.UserBackgroundImages.Select(x => new UserBackgroundImageResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                Url = x.Url,
                CreatedAt = x.CreatedAt,
            });
        }

        public async Task<List<UserBackgroundImageResponse>> GetResponsesByUserIdAsync(string userId)
        {
            return await BuildResponseQuery()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}

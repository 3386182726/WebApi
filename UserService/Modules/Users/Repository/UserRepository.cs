using System.Reflection;
using Common.Pagination;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Repository
{
    public class UserRepository(UserDbContext dbContext) : IUserRepository
    {
        public enum UserSortField
        {
            UserName,
            Email,
            Birthday,
        }

        public async Task<PagedResult<UserResultDto>> GetUsersAsync(PagedRequest request)
        {
            int skip = (request.Page - 1) * request.PageSize;

            var query =
                from u in dbContext.Users
                join ur in dbContext.UserRoles on u.Id equals ur.UserId into urj
                from ur in urj.DefaultIfEmpty()
                join r in dbContext.Roles on ur.RoleId equals r.Id into rj
                from r in rj.DefaultIfEmpty()
                group r by new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Name,
                    u.Birthday,
                } into g
                select new UserResultDto
                {
                    Id = g.Key.Id,
                    UserName = g.Key.UserName,
                    Email = g.Key.Email,
                    Name = g.Key.Name,
                    Birthday = g.Key.Birthday,
                    Roles = g.Where(x => x != null).Select(x => x.Name).ToList(),
                };

            // 1️⃣ 搜索过滤
            if (!string.IsNullOrEmpty(request.Search))
            {
                string lowerSearch = request.Search.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(lowerSearch)
                    || (u.Name != null && u.Name.ToLower().Contains(lowerSearch))
                    || (u.Email != null && u.Email.ToLower().Contains(lowerSearch))
                );
            }

            // 2️⃣ 排序
            if (!string.IsNullOrEmpty(request.SortField))
            {
                var prop = typeof(UserResultDto).GetProperty(
                    request.SortField,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );
                if (prop != null)
                {
                    query = request.SortDesc
                        ? query.OrderByDescending(u => EF.Property<object>(u, prop.Name))
                        : query.OrderBy(u => EF.Property<object>(u, prop.Name));
                }
            }
            else
            {
                // 默认排序
                query = query.OrderBy(u => u.UserName);
            }

            var total = query.Count();
            var items = await query.Skip(skip).Take(request.PageSize).ToListAsync();

            return new PagedResult<UserResultDto>
            {
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize,
                Items = items,
            };
        }
    }
}

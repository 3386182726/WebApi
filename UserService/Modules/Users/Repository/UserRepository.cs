using Common.EFCore;
using System.Linq.Expressions;
using UserService.Data;
using UserService.Modules.Users.Dto;

namespace UserService.Modules.Users.Repository
{
    /// <summary>
    /// 纯数据库访问：只负责 User 表 + Role + Organization 的 EF 查询，
    /// 跨服务图片组装由 UserService 负责。
    /// </summary>
    public class UserRepository(UserDbContext dbContext)
        : RepositoryBase<User, UserResponse, UserDbContext>(dbContext)
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
    }
}

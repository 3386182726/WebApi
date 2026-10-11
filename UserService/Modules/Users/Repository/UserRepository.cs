using Common.EFCore;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Modules.Users.Dto;
using System.Linq.Expressions;

namespace UserService.Modules.Users.Repository
{
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
                join ur in DbContext.UserRoles on u.Id equals ur.UserId into urj
                from ur in urj.DefaultIfEmpty()
                join r in DbContext.Roles on ur.RoleId equals r.Id into rj
                from r in rj.DefaultIfEmpty()
                group r by new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.Name,
                    u.Birthday,
                } into g
                select new UserResponse
                {
                    Id = g.Key.Id,
                    UserName = g.Key.UserName,
                    Email = g.Key.Email,
                    Name = g.Key.Name,
                    Birthday = g.Key.Birthday,
                    Roles = g.Where(x => x != null).Select(x => x.Name).ToList(),
                };
        }
    }
}

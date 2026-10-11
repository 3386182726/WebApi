using Common.Abstractions;
using Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Common.EFCore
{
    public abstract class RepositoryBase<TEntity, TResponse, TDbContext>(TDbContext dbContext)
        : IRepository<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext = dbContext;
        protected readonly DbSet<TEntity> Set = dbContext.Set<TEntity>();

        /// <summary>允许搜索和排序的字段白名单（默认空 = 不允许）</summary>
        protected virtual HashSet<string> AllowedFields { get; } = [];

        protected virtual Expression<Func<TResponse, object>> DefaultOrderBy
            => x => EF.Property<object>(x, "Id");

        protected abstract IQueryable<TResponse> BuildResponseQuery();

        public virtual async Task<TEntity?> GetByIdAsync(string id)
            => await Set.FindAsync(id);

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
            => await Set.AsNoTracking().ToListAsync();

        public virtual async Task<TResponse?> GetResponseByIdAsync(string id)
        {
            return await BuildResponseQuery()
                .Where(n => EF.Property<object>(n, "Id").Equals(id))
                .FirstOrDefaultAsync();
        }

        public virtual async Task<Pagination.PagedResult<TResponse>> GetListAsync(PagedRequest request)
        {
            var query = BuildResponseQuery();

            // 搜索
            if (request.Conditions?.Count > 0)
            {
                foreach (var (field, value) in request.Conditions)
                {
                    if (string.IsNullOrWhiteSpace(value) || !AllowedFields.Contains(field,StringComparer.OrdinalIgnoreCase)) continue;

                    var escaped = value.Replace("%", "\\%").Replace("_", "\\_");
                    query = query.Where($"{field}.Contains(@0)", escaped);
                }
            }

            // 排序
            query = !string.IsNullOrWhiteSpace(request.SortField) && AllowedFields.Contains(request.SortField)
                ? query.OrderBy($"{request.SortField} {(request.SortDesc ? "descending" : "ascending")}")
                : query.OrderBy(DefaultOrderBy);

            var total = await query.CountAsync();
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

            return new Pagination.PagedResult<TResponse>
            {
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize,
                Items = items,
            };
        }

        public virtual void Create(TEntity entity) => Set.Add(entity);
        public virtual void Update(TEntity entity) => Set.Update(entity);
        public virtual void Remove(TEntity entity) => Set.Remove(entity);
        public virtual async Task<int> SaveChangesAsync() => await DbContext.SaveChangesAsync();
    }
}

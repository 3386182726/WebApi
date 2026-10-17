using Common.Pagination;
using System.Linq.Expressions;

namespace Common.Abstractions
{
    public interface IRepository<TEntity, TResponse>
    where TEntity : class
    where TResponse : class
    {
        Task<TEntity?> GetByIdAsync(string id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TResponse?> GetResponseByIdAsync(string id);
        Task<PagedResult<TResponse>> GetListAsync(PagedRequest pagedRequest);
        Task<List<TResponse>> QueryResponsesAsync(Expression<Func<TResponse, bool>> predicate);
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<int> SaveChangesAsync();
    }
}

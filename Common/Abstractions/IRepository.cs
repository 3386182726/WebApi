using Common.Pagination;

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
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<int> SaveChangesAsync();
    }
}

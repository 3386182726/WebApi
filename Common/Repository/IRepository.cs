using Common.Pagination;

namespace Common.Repository
{
    public interface IRepository<T,T2> where T : class
        where T2 : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<PagedResult<T2>> GetListAsync(PagedRequest pagedRequest);
        void Create(T entity);
        void Update(T entity);
        void Remove(T entity);
        public Task<int> SaveChangesAsync();
    }
}

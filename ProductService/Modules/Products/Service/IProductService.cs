using Common.Pagination;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;

namespace ProductService.Modules.Products.Service
{
    public interface IProductService
    {
        public Task<Product?> GetAsync(string id);
        public Task<PagedResult<ProductResultDto>> GetListAsync(PagedRequest request);
        public void Create(Product product);
        public void Update(Product product);
        public void Remove(Product product);
        public Task<int> SaveChangesAsync();
    }
}

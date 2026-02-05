using Common.Pagination;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;
using ProductService.Modules.Products.Repository;

namespace ProductService.Modules.Products.Service
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public Task<Product?> GetAsync(string id)
        {
            return productRepository.GetAsync(id);
        }

        public Task<PagedResult<ProductResultDto>> GetListAsync(PagedRequest request)
        {
            return productRepository.GetListAsync(request);
        }

        public void Create(Product product)
        {
            productRepository.Create(product);
        }

        public void Update(Product product)
        {
            productRepository.Update(product);
        }

        public void Remove(Product product)
        {
            productRepository.Remove(product);
        }

        public Task<int> SaveChangesAsync()
        {
            return productRepository.SaveChangesAsync();
        }
    }
}

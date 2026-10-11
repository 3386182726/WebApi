using Common.Abstractions;
using Common.ExceptionHandling.CustomExceptions;
using Common.Pagination;
using Microsoft.EntityFrameworkCore;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;

namespace ProductService.Modules.Products.Service
{
    public class ProductService(IRepository<Product, ProductResponse> productRepository) : IProductService
    {
        public Task<Product?> GetAsync(string id)
        {
            return productRepository.GetByIdAsync(id);
        }

        public Task<PagedResult<ProductResponse>> GetListAsync(PagedRequest request)
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

        public async Task<SeckillResponse> Seckill(string productId, string UserId)
        {
            try
            {
                var product = await productRepository.GetByIdAsync(productId);
                if (product != null)
                {
                    if (product.Count > 0)
                    {
                        product.Count--;
                        await productRepository.SaveChangesAsync();
                    }
                    else
                    {
                        return new SeckillResponse() { Message = "库存不足." };
                    }
                }
                else
                {
                    return new SeckillResponse() { Message = "产品不存在." };
                }
                return new SeckillResponse() { Success = true };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new SeckillResponse() { Message = "购买失败，数据已被他人修改." + ex.Message };
            }
        }

        public Task<int> SaveChangesAsync()
        {
            return productRepository.SaveChangesAsync();
        }
    }
}

using System.Reflection;
using Common.Pagination;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;

namespace ProductService.Modules.Products.Repository
{
    public class ProductRepository(ProductDbContext dbContext) : IProductRepository
    {
        public async Task<Product?> GetAsync(string id)
        {
            return await dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<ProductResultDto>> GetListAsync(PagedRequest request)
        {
            int skip = (request.Page - 1) * request.PageSize;

            var query =
                from u in dbContext.Products
                select new ProductResultDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Category = u.Category,
                    Status = u.Status,
                    CreatedAt = u.CreatedAt,
                    Imgs = u.Imgs,
                    Price = u.Price,
                    Count = u.Count,
                };

            // 1️⃣ 搜索过滤
            if (!string.IsNullOrEmpty(request.Search))
            {
                string lowerSearch = request.Search.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(lowerSearch)
                    || (u.Name != null && u.Name.ToLower().Contains(lowerSearch))
                );
            }

            // 2️⃣ 排序
            if (!string.IsNullOrEmpty(request.SortField))
            {
                var prop = typeof(ProductResultDto).GetProperty(
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
                query = query.OrderBy(u => u.Name);
            }

            var total = query.Count();
            var items = await query.Skip(skip).Take(request.PageSize).ToListAsync();

            return new PagedResult<ProductResultDto>
            {
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize,
                Items = items,
            };
        }

        public void Create(Product product)
        {
            dbContext.Products.Add(product);
        }

        public void Update(Product product)
        {
            dbContext.Products.Update(product);
        }

        public void Remove(Product product)
        {
            dbContext.Products.Remove(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}

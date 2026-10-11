using Common.EFCore;
using ProductService.Data;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;
using System.Linq.Expressions;

namespace ProductService.Modules.Products.Repository
{
    public class ProductRepository(ProductDbContext dbContext)
        : RepositoryBase<Product, ProductResponse, ProductDbContext>(dbContext)
    {
        protected override HashSet<string> AllowedFields { get; } = ["Name"];

        protected override Expression<Func<ProductResponse, object>> DefaultOrderBy
            => x => x.Name;

        protected override IQueryable<ProductResponse> BuildResponseQuery()
        {
            return
                from u in DbContext.Products
                select new ProductResponse
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
        }
    }
}

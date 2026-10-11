using Common.Abstractions;
using Common.Service;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;
using ProductService.Modules.Products.Repository;
using ProductService.Modules.Products.Service;

namespace ProductService.Modules.Products
{
    public static class ProductServiceCollectionExtensions
    {
        public static IServiceCollection AddProductService(this IServiceCollection services)
        {
            services.AddScoped<IProductService, Service.ProductService>();
            services.AddScoped<IRepository<Product, ProductResponse>, ProductRepository>();
            services.AddScoped<IUploadService, UploadService>();
            return services;
        }
    }
}

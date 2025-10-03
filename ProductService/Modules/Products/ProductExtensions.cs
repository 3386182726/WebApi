using Common.Service;
using ProductService.Modules.Products.Repository;
using ProductService.Modules.Products.Service;

namespace ProductService.Modules.Products
{
    public static class ProductExtensions
    {
        public static IServiceCollection AddProductService(this IServiceCollection services)
        {
            services.AddScoped<IProductService, Service.ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUploadService, UploadService>();
            return services;
        }
    }
}

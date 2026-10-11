using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;

namespace ProductService.Modules
{
    public static class ProductMappingExtensions
    {
        public static void UpdateFrom(this Product product, ProductRequest dto)
        {
            product.Name = dto.Name;
            product.Category = dto.Category;
            product.Status = dto.Status;
            product.Imgs = dto.Imgs;
            product.Count = dto.Count;
            product.Price = dto.Price;
        }
    }
}

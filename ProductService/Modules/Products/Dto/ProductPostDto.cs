using ProductService.Modules.Products.Model;

namespace ProductService.Modules.Products.Dto
{
    public class ProductPostDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public ProductCategory Category { get; set; }
        public ProductStatus Status { get; set; }
        public string[]? Imgs { get; set; }
    }
}

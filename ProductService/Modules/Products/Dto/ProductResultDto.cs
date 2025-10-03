using ProductService.Modules.Products.Model;

namespace ProductService.Modules.Products.Dto
{
    public class ProductResultDto
    {
        public string Id { get; set; } = null!;
        public required string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public ProductCategory Category { get; set; }
        public ProductStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[]? Imgs { get; set; }

    }
}

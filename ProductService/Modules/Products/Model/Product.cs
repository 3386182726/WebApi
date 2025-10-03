using System.Text.Json.Serialization;

namespace ProductService.Modules.Products.Model
{
    public class Product
    {
        public string Id { get; set; } = null!;
        public required string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductCategory Category { get; set; }
        public ProductStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[]? Imgs { get; set; }
    }
}

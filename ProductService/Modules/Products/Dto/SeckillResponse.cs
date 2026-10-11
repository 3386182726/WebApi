namespace ProductService.Modules.Products.Dto;

public class SeckillResponse
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }   // 如 "OUT_OF_STOCK"
    public string? Message { get; set; }
}

namespace ProductService.Modules.Products.Model
{
    public enum ProductStatus
    {
        Draft = 0, // 未上架
        Available = 1, // 可购买
        Sold = 2, // 已售出
        Removed = 3, // 已下架/删除
    }
}

namespace Common.Pagination
{
    public class PagedRequest
    {
        public int Page { get; set; } = 1; // 当前页，默认 1
        public int PageSize { get; set; } = 10; // 每页数量，默认 10
        public string? Search { get; set; } // 可选搜索关键字
        public string? SortField { get; set; } // 可选排序字段
        public bool SortDesc { get; set; } = false; // 是否降序
    }
}

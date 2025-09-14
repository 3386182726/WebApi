namespace WebApi.Dtos
{
    public class PagedResult<T>
    {
        public int Total { get; set; } // 总记录数
        public int Page { get; set; } // 当前页
        public int PageSize { get; set; } // 每页数量
        public List<T> Items { get; set; } = []; // 当前页数据
    }
}

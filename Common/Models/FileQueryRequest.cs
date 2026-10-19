namespace Common.Models
{
    /// <summary>
    /// FileService /api/file/query 批量查询接口的请求体。
    /// 所有需要跨服务调 FileService 的业务服务共用此 DTO。
    /// </summary>
    public class FileQueryRequest
    {
        public required string OwnerType { get; set; }
        public required List<string> OwnerIds { get; set; } = [];
        public string? Category { get; set; }
        public string? Type { get; set; }
    }
}

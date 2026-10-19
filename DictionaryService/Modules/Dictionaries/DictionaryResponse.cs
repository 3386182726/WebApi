using Common.Models;

namespace DictionaryService.Modules.Dictionaries
{
    public class DictionaryResponse
    {
        public required string Id { get; set; }
        public required string DictType { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? ParentId { get; set; }
        public int Sort { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }

        /// <summary>从 FileService 组装回来的展示图片</summary>
        public List<AppFileResponse> Images { get; set; } = [];

        /// <summary>子节点（树构建时填充）</summary>
        public List<DictionaryResponse> Children { get; set; } = [];
    }
}

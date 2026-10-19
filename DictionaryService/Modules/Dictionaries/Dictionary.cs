namespace DictionaryService.Modules.Dictionaries
{
    /// <summary>
    /// 选项型字典项：按 DictType 分组，每组包含多个 Code/Name 选项，
    /// 支持同一 DictType 内的多层级结构（ParentId 自引用）。
    /// 例：DictType="Country" 下，顶层是国家，子级是省/州，再下是城市。
    /// 展示图片走 FileService（OwnerType=Dictionary, OwnerId=Id）。
    /// </summary>
    public class Dictionary
    {
        public string Id { get; set; } = null!;

        /// <summary>字典类型分组键，如 "Gender"、"Education"、"Country"</summary>
        public required string DictType { get; set; }

        /// <summary>选项编码，同一 DictType 下唯一</summary>
        public required string Code { get; set; }

        /// <summary>选项显示名</summary>
        public required string Name { get; set; }

        /// <summary>父级 Id，null 表示根节点（同一 DictType 内）</summary>
        public string? ParentId { get; set; }

        /// <summary>排序，数字小的在前</summary>
        public int Sort { get; set; }

        /// <summary>是否启用</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>可选描述</summary>
        public string? Description { get; set; }
    }
}

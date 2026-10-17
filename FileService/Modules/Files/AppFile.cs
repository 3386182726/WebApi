namespace FileService.Modules.Files
{
    /// <summary>
    /// 通用图片/文件表
    /// OwnerType 区分归属（User / Organization / Product / Order ...）
    /// Category 区分资源种类（Image / File / Zip / Video）
    /// Type 区分业务类型（Avatar / Photo / Background / Attachment 等）
    /// </summary>
    public class AppFile
    {
        public string Id { get; set; } = null!;

        /// <summary>所有者类型：User / Organization / Product / Order 等</summary>
        public required string OwnerType { get; set; }

        /// <summary>所有者 Id</summary>
        public required string OwnerId { get; set; }

        /// <summary>资源种类：Image / File / Zip / Video</summary>
        public required string Category { get; set; }

        /// <summary>业务类型：Avatar / Photo / Background / Attachment 等</summary>
        public required string Type { get; set; }

        public required string Url { get; set; }

        public required string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

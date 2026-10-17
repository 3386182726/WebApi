namespace FileService.Modules.Files
{
    /// <summary>
    /// AppFile 通用常量（OwnerType / Category / Type）
    /// </summary>
    public static class AppFileConstants
    {
        // OwnerType — 所有者类型
        public const string OwnerTypeUser = "User";
        public const string OwnerTypeOrganization = "Organization";
        public const string OwnerTypeProduct = "Product";
        public const string OwnerTypeOrder = "Order";

        // Category — 资源种类，决定中间目录
        public const string CategoryImage = "Image";
        public const string CategoryFile = "File";
        public const string CategoryZip = "Zip";
        public const string CategoryVideo = "Video";

        // Type — 业务类型，决定末级目录和语义
        public const string TypeAvatar = "Avatar";
        public const string TypePhoto = "Photo";
        public const string TypeBackground = "Background";
        public const string TypeAttachment = "Attachment";
    }
}

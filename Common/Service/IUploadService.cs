namespace Common.Service
{
    public interface IUploadService
    {
        /// <summary>
        /// 上传单张文件
        /// </summary>
        /// <param name="file">文件流</param>
        /// <param name="ownerType">所有者类型（User / Organization）</param>
        /// <param name="category">资源种类（Image / File / Zip / Video），决定中间目录</param>
        /// <param name="type">业务类型（Avatar / Photo / Background），决定末级目录</param>
        /// <returns>文件 URL</returns>
        Task<string> UploadAsync(IFormFile file, string? ownerType = null, string? category = null, string? type = null);

        /// <summary>
        /// 根据相对 URL 删除已上传的文件
        /// </summary>
        /// <param name="fileUrl">上传时返回的相对 URL，例如 /uploads/images/xxx.png</param>
        void Delete(string? fileUrl);
    }
}

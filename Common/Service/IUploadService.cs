namespace Common.Service
{
    public interface IUploadService
    {
        /// <summary>
        /// 上传单张图片
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns>图片 URL</returns>
        Task<string> UploadAsync(IFormFile file);

        /// <summary>
        /// 上传多张图片
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <returns>图片 URL 列表</returns>
        Task<IEnumerable<string>> UploadManyAsync(IEnumerable<IFormFile> files);

        /// <summary>
        /// 根据相对 URL 删除已上传的文件
        /// </summary>
        /// <param name="fileUrl">上传时返回的相对 URL，例如 /uploads/images/xxx.png</param>
        void Delete(string? fileUrl);
    }
}

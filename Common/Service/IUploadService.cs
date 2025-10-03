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
    }
}

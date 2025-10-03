﻿namespace Common.Service
{
    public class UploadService(IWebHostEnvironment env) : IUploadService
    {
        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("文件不能为空");

            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists("/uploads"))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}"; // 返回相对 URL，前端可以拼接域名
        }

        public async Task<IEnumerable<string>> UploadManyAsync(IEnumerable<IFormFile> files)
        {
            var urls = new List<string>();
            foreach (var file in files)
            {
                var url = await UploadAsync(file);
                urls.Add(url);
            }
            return urls;
        }
    }
}

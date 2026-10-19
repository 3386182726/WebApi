using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Common.Service
{
    public class UploadService(IWebHostEnvironment env) : IUploadService
    {
        public async Task<string> UploadAsync(IFormFile file, string? ownerType = null, string? category = null, string? type = null)
        {
            Console.WriteLine($"Saved file: 开始保存图片"); // 🔹 日志输出
            if (file == null || file.Length == 0)
                throw new ArgumentException("文件不能为空");

            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists("/uploads"))
                Directory.CreateDirectory(uploadsFolder);

            // 组合子目录：ownerType/category/type，逐层兜底
            var subDir = file.ContentType.StartsWith("image/") ? "images" : "videos";
            if (!string.IsNullOrEmpty(ownerType) && !string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(type))
                subDir = Path.Combine(ownerType, category, type);
            else if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(type))
                subDir = Path.Combine(category, type);
            else if (!string.IsNullOrEmpty(type))
                subDir = type;

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, subDir, fileName);
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            try {
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            }
            catch(Exception e)
            {
                string s = e.Message;
            }

            Console.WriteLine($"Saved file: {filePath}"); // 🔹 日志输出
            return $"/uploads/{subDir.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}";
        }

        public void Delete(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            var webRoot = env.WebRootPath ?? env.ContentRootPath;
            var relativePath = fileUrl
                .Replace('/', Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(webRoot, relativePath));

            // 防止路径越界删除 webRoot 之外的文件
            if (
                !fullPath.StartsWith(
                    Path.GetFullPath(webRoot) + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                return;

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}

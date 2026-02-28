using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoteService.Test
{
    public class NotesControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        public readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Upload_ShouldReturnUrl()
        {
            // 1️⃣ 模拟文件
            var content = new MultipartFormDataContent();

            // 模拟图片文件内容
            string base64 =
"iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=";
            byte[] imageBytes = Convert.FromBase64String(base64);
            var imageContent = new ByteArrayContent(imageBytes);

            // 设置 MIME 类型
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            // 参数名必须和 Controller 的 IFormFile 参数一致
            content.Add(imageContent, "file", "test.png");

            // 2️⃣ 发送 POST 请求
            var response = await _client.PostAsync("/api/note/upload", content);

            response.EnsureSuccessStatusCode();

            // 3️⃣ 解析返回 JSON
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            Assert.NotNull(result);
            Assert.True(result.ContainsKey("url"));
            Assert.False(string.IsNullOrEmpty(result["url"]));
        }
    }
}

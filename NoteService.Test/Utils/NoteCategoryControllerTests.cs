using NoteService.Modules.Notes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoteService.Test.Utils
{
    public class NoteCategoryControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        [Fact]
        public async Task SaveNoteCategory_ShouldReturnOK()
        {
            var _client = factory.CreateClientWithTestAuth();
            var request = new NoteCategoryRequest
            {
                Name = "Test NoteCategory"
            };

            // 2️⃣ 发送 POST 请求
            var result = await _client.PostAsJsonAsync("/api/noteCategory", request);
            var content = await result.Content.ReadAsStringAsync();
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadFromJsonAsync<NoteCategoryResponse>();
            Assert.NotNull(response);
        }
    }
}

using Common.Models;
using Common.Pagination;
using Flurl;
using Flurl.Http;

namespace DictionaryService.Modules.Dictionaries
{
    /// <summary>
    /// 字典业务：EF 查询委托给 DictionaryRepository，跨服务图片组装自己做。
    /// </summary>
    public interface IDictService
    {
        Task<DictionaryResponse?> GetByIdAsync(string id);
        Task<Common.Pagination.PagedResult<DictionaryResponse>> GetPagedAsync(PagedRequest request);

        /// <summary>按 DictType 获取扁平列表（不分页，适合下拉框等）</summary>
        Task<List<DictionaryResponse>> GetByTypeAsync(string dictType, bool? isActive = null);

        /// <summary>批量按多个 DictType 获取扁平列表</summary>
        Task<List<DictionaryResponse>> GetByTypesAsync(IEnumerable<string> dictTypes, bool? isActive = null);

        /// <summary>按 DictType 获取层级树（适合省市区、分类等）</summary>
        Task<List<DictionaryResponse>> GetTreeByTypeAsync(string dictType, bool? isActive = null);
    }

    public class DictService(
        IDictionaryRepository repository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    ) : IDictService
    {
        public async Task<DictionaryResponse?> GetByIdAsync(string id)
        {
            var item = await repository.GetByIdResponseAsync(id);
            if (item != null) await FillImagesAsync([item]);
            return item;
        }

        public async Task<Common.Pagination.PagedResult<DictionaryResponse>> GetPagedAsync(PagedRequest request)
        {
            var result = await repository.GetPagedAsync(request);
            if (result.Items.Count > 0)
                await FillImagesAsync(result.Items);
            return result;
        }

        public async Task<List<DictionaryResponse>> GetByTypeAsync(string dictType, bool? isActive = null)
        {
            var items = await repository.GetByTypeAsync(dictType, isActive);
            if (items.Count > 0) await FillImagesAsync(items);
            return items;
        }

        public async Task<List<DictionaryResponse>> GetByTypesAsync(IEnumerable<string> dictTypes, bool? isActive = null)
        {
            var items = await repository.GetByTypesAsync(dictTypes, isActive);
            if (items.Count > 0) await FillImagesAsync(items);
            return items;
        }

        public async Task<List<DictionaryResponse>> GetTreeByTypeAsync(string dictType, bool? isActive = null)
        {
            var all = await repository.GetByTypeAsync(dictType, isActive);
            if (all.Count == 0) return [];

            // 一次性组装所有图片，避免递归里重复调 FileService
            await FillImagesAsync(all);

            // 内存构建树
            var lookup = all.ToDictionary(x => x.Id);
            var roots = new List<DictionaryResponse>();

            foreach (var item in all)
            {
                if (string.IsNullOrEmpty(item.ParentId) || !lookup.TryGetValue(item.ParentId, out var parent))
                {
                    roots.Add(item);
                }
                else
                {
                    parent.Children.Add(item);
                }
            }

            return roots;
        }

        private async Task FillImagesAsync(IEnumerable<DictionaryResponse> items)
        {
            var ids = items.Select(i => i.Id).ToList();
            if (ids.Count == 0) return;

            var baseUrl = configuration["FileService:BaseUrl"] ?? "http://fileservice:8080";
            using var httpClient = httpClientFactory.CreateClient();

            var query = new FileQueryRequest
            {
                OwnerType = AppFileConstants.OwnerTypeDictionary,
                OwnerIds = ids,
                Category = AppFileConstants.CategoryImage,
            };

            var allFiles = await baseUrl
                .AppendPathSegments("api", "file", "query")
                .PostJsonAsync(query)
                .ReceiveJson<List<AppFileResponse>>() ?? [];

            var grouped = allFiles.GroupBy(f => f.OwnerId);

            foreach (var item in items)
            {
                item.Images = grouped.FirstOrDefault(g => g.Key == item.Id)?.ToList() ?? [];
            }
        }
    }
}

using Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace DictionaryService.Modules.Dictionaries
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController(
        IDictService service,
        IDictionaryRepository repository
    ) : ControllerBase
    {
        /// <summary>后台管理分页查询（带搜索、排序，带图片）</summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
        {
            var result = await service.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>按 DictType 获取扁平列表（前端下拉框用，带图片，不分页）</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string dictType, [FromQuery] bool? isActive = null)
        {
            if (string.IsNullOrWhiteSpace(dictType))
                return BadRequest("dictType 必填");
            var items = await service.GetByTypeAsync(dictType, isActive);
            return Ok(items);
        }

        /// <summary>按 DictType 获取层级树（省市区、分类等带父子关系的场景）</summary>
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree([FromQuery] string dictType, [FromQuery] bool? isActive = null)
        {
            if (string.IsNullOrWhiteSpace(dictType))
                return BadRequest("dictType 必填");
            var items = await service.GetTreeByTypeAsync(dictType, isActive);
            return Ok(items);
        }

        /// <summary>批量按多个 DictType 获取（带图片，不分页）</summary>
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string dictTypes, [FromQuery] bool? isActive = null)
        {
            var types = dictTypes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var items = await service.GetByTypesAsync(types, isActive);
            return Ok(items);
        }

        /// <summary>按 Id 获取单条（带图片）</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        /// <summary>新增</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Dictionary dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            repository.Create(dto);
            await repository.SaveChangesAsync();
            return Ok(dto);
        }

        /// <summary>更新</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Dictionary dto)
        {
            if (id != dto.Id) return BadRequest("Id 不匹配");
            var existing = await repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.DictType = dto.DictType;
            existing.Code = dto.Code;
            existing.Name = dto.Name;
            existing.ParentId = dto.ParentId;
            existing.Sort = dto.Sort;
            existing.IsActive = dto.IsActive;
            existing.Description = dto.Description;

            repository.Update(existing);
            await repository.SaveChangesAsync();

            // 返回带图片的 DTO
            var refreshed = await service.GetByIdAsync(id);
            return Ok(refreshed);
        }

        /// <summary>删除</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            repository.Remove(entity);
            await repository.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}

using Common.Models;
using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileService.Modules.Files
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController(
        IAppFileRepository appFileRepository,
        IUploadService uploadService
    ) : ControllerBase
    {
        private static readonly HashSet<string> ValidOwnerTypes = [
            AppFileConstants.OwnerTypeUser,
            AppFileConstants.OwnerTypeOrganization,
            AppFileConstants.OwnerTypeProduct,
            AppFileConstants.OwnerTypeOrder,
        ];

        private static readonly HashSet<string> ValidCategories = [
            AppFileConstants.CategoryImage,
            AppFileConstants.CategoryFile,
            AppFileConstants.CategoryZip,
            AppFileConstants.CategoryVideo,
        ];

        /// <summary>上传图片 / 文件（需鉴权）</summary>
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(
            [FromQuery] string ownerType,
            [FromQuery] string? ownerId,
            [FromQuery] string category,
            [FromQuery] string type,
            [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("文件不能为空");

            if (!ValidOwnerTypes.Contains(ownerType))
                return BadRequest($"ownerType 必须是 {string.Join("/", ValidOwnerTypes)}");

            if (!ValidCategories.Contains(category))
                return BadRequest($"category 必须是 {string.Join("/", ValidCategories)}");

            var effectiveOwnerId = ownerId;
            if (ownerType == AppFileConstants.OwnerTypeUser && string.IsNullOrEmpty(ownerId))
            {
                effectiveOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(effectiveOwnerId))
                    return Unauthorized();
            }
            else if (string.IsNullOrEmpty(ownerId))
            {
                return BadRequest("ownerId 必填（非 User 场景）");
            }

            var url = await uploadService.UploadAsync(file, ownerType, category, type);

            var entity = new AppFile
            {
                OwnerType = ownerType,
                OwnerId = effectiveOwnerId!,
                Category = category,
                Type = type,
                Url = url,
                Name = file.FileName,
            };
            appFileRepository.Create(entity);
            await appFileRepository.SaveChangesAsync();

            return Ok(new { id = entity.Id, url });
        }

        /// <summary>批量查询（按 OwnerType + 多个 OwnerId 一次性返回，供其他微服务内部调用）</summary>
        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] FileQueryRequest request)
        {
            var files = await appFileRepository.GetByOwnersAsync(
                request.OwnerType,
                request.OwnerIds,
                request.Category);

            return Ok(files.Select(ToResponse));
        }

        /// <summary>按所有者查询文件列表</summary>
        [HttpGet]
        public async Task<IActionResult> GetByOwner(
            [FromQuery] string ownerType,
            [FromQuery] string? ownerId,
            [FromQuery] string? category = null,
            [FromQuery] string? type = null)
        {
            if (!ValidOwnerTypes.Contains(ownerType))
                return BadRequest($"ownerType 必须是 {string.Join("/", ValidOwnerTypes)}");

            var effectiveOwnerId = ownerId;
            if (ownerType == AppFileConstants.OwnerTypeUser && string.IsNullOrEmpty(ownerId))
            {
                effectiveOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(effectiveOwnerId))
                    return Unauthorized();
            }
            else if (string.IsNullOrEmpty(ownerId))
            {
                return BadRequest("ownerId 必填（非 User 场景）");
            }

            var files = await appFileRepository.GetByOwnerAsync(ownerType, effectiveOwnerId!, category, type);
            return Ok(files.Select(ToResponse));
        }

        /// <summary>删除文件（需鉴权）</summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("id 不能为空");

            var entity = await appFileRepository.GetByIdAsync(id);
            if (entity == null)
                return NotFound("文件不存在");

            if (entity.OwnerType == AppFileConstants.OwnerTypeUser)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || entity.OwnerId != userId)
                    return Forbid();
            }

            var url = entity.Url;
            appFileRepository.Remove(entity);
            await appFileRepository.SaveChangesAsync();
            uploadService.Delete(url);

            return Ok("删除成功");
        }

        static AppFileResponse ToResponse(AppFile f) => new()
        {
            Id = f.Id,
            OwnerId = f.OwnerId,
            Category = f.Category,
            Type = f.Type,
            Url = f.Url,
            Name = f.Name,
            CreatedAt = f.CreatedAt,
        };
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Modules.Users.Service;

namespace UserService.Modules.Users
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Authorize]
    public class UserBackgroundImageController(IUserBackgroundImageService backgroundImageService)
        : ControllerBase
    {
        // GET: api/user/userbackgroundimage/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await backgroundImageService.GetAllByUserIdAsync(userId);
            return Ok(result);
        }

        // POST: api/user/userbackgroundimage/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("文件不能为空");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("仅支持上传图片文件");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await backgroundImageService.UploadAsync(file, userId);
            return Ok(result);
        }

        // DELETE: api/user/userbackgroundimage/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var image = await backgroundImageService.GetByIdAsync(id);
            if (image == null)
                return NotFound("背景图不存在");

            if (image.UserId != userId)
                return Forbid();

            await backgroundImageService.DeleteAsync(image);
            return NoContent();
        }
    }
}

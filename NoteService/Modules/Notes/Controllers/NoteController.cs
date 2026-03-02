using Common.Pagination;
using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using System.Security.Claims;

namespace NoteService.Modules.Notes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController(IUploadService uploadService,IService<Note, NoteResponse> service) :ControllerBase
    {
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var url = await uploadService.UploadAsync(file);
                return Ok(new {
                    errno= 0, 
                data=new  {
                                    url= url, // 图片 src ，必须
                    alt="", // 图片描述文字，非必须
                    href="" // 图片的链接，非必须
                }
                            });
                        }
            catch (Exception ex) {
                return Ok(new
                {
                    errno = 1,
                    message = "失败信息:"+ex.Message
                });
            }
            
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<NoteResponse>>> GetNotes([FromQuery] PagedRequest pagedRequest)
        {
            var result = await service.GetListAsync(pagedRequest);
            return Ok(result );
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetNote(string id)
        {
            var result = await service.GetByIdAsync(id);
            return Ok(result);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNote(NoteRequest noteRequest)
        {
            // 获取用户 ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            Console.WriteLine($"User ID: {userId}");
            var note = new Note()
            {
                Name = noteRequest.Name,
                Category = noteRequest.Category,
                Content = noteRequest.Content,
                CreaterId = userId??string.Empty,
                CreatedAt = DateTime.UtcNow,
            };
            if (string.IsNullOrEmpty(noteRequest.Id))
            {
                service.Create(note);
            }
            else
            {
                note.Id = noteRequest.Id!;
                service.Update(note);
            }
            await service.SaveChangesAsync();
            return Ok("保存成功");
        }

        [HttpDelete]
        [Route("notes/{id}")]
        public async Task<IActionResult> DeleteNoteAsync(string id) {
            var note = await service.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            service.Remove(note);
            await service.SaveChangesAsync();
            return NoContent();
        }
    }
}

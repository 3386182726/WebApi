using Common.Pagination;
using Common.Service;
using Microsoft.AspNetCore.Mvc;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;

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
            var url = await uploadService.UploadAsync(file);
            return Ok(new { url});
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes(PagedRequest pagedRequest)
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
        public async Task<IActionResult> CreateNote(NoteRequest noteRequest)
        {
            var userId = Request.Headers["X-UserId"].FirstOrDefault();
            var note = new Note()
            {
                Name = noteRequest.Name,
                Category = noteRequest.Category,
                Content = noteRequest.Content,
                CreaterId = userId??string.Empty
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

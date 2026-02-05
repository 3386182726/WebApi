using Common.Pagination;
using Common.Service;
using Microsoft.AspNetCore.Mvc;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteCategoryController(IService<NoteCategory, NoteCategoryRequest> service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNoteCategories(PagedRequest pagedRequest)
        {
            var result = await service.GetListAsync(pagedRequest);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteCategory(string id)
        {
            var result = await service.GetByIdAsync(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post(NoteCategoryRequest noteRequest)
        {
            var noteCategory = new NoteCategory()
            {
                Name = noteRequest.Name
            };
            if (string.IsNullOrEmpty(noteRequest.Id))
            {
                service.Create(noteCategory);
            }
            else
            {
                noteCategory.Id = noteRequest.Id!;
                service.Update(noteCategory);
            }
            await service.SaveChangesAsync();
            return Ok("保存成功");
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(string id) {
            var noteCategory = await service.GetByIdAsync(id);
            if (noteCategory == null)
                return NotFound();

            service.Remove(noteCategory);
            await service.SaveChangesAsync();
            return NoContent();
        }
    }
}

using Common.Pagination;
using Common.Service;
using Microsoft.AspNetCore.Mvc;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using NoteService.Modules.Notes.Service;

namespace NoteService.Modules.Notes.Controllers
{
    [ApiController]
    [Route("api/note/[controller]")]
    public class NoteCategoryController(INoteCategoryService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNoteCategories([FromQuery] PagedRequest pagedRequest)
        {
            var result = await service.GetListAsync(pagedRequest);
            return Ok(result);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetNoteCategories()
        {
            var result = await service.GetNoteCategoriesAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteCategory(string id)
        {
            var result = await service.GetResponseByIdAsync(id);
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
            var response = NoteCategoryResponse.FromModel(noteCategory);
            return Ok(response);
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

using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ArcaiProject.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _service;

        public TagsController(ITagService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll([FromQuery] PagingParameters pagingParameters)
        {
            var pagedList = await _service.GetAllTagsAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedList.Metadata));
            return Ok(pagedList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetById(int id)
        {
            var item = await _service.GetTagByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.AddTagAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "ID in URL and body do not match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _service.UpdateTagAsync(id, dto);
                if (updated == null) return NotFound(new { message = "Tag not found." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteTagAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

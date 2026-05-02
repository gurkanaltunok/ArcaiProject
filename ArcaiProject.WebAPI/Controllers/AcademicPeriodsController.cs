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
    public class AcademicPeriodsController : ControllerBase
    {
        private readonly IAcademicPeriodService _service;

        public AcademicPeriodsController(IAcademicPeriodService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicPeriodDto>>> GetAll([FromQuery] PagingParameters pagingParameters)
        {
            var pagedList = await _service.GetAllAcademicPeriodsAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedList.Metadata));
            return Ok(pagedList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicPeriodDto>> GetById(int id)
        {
            var item = await _service.GetAcademicPeriodByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<AcademicPeriodDto>> Create([FromBody] CreateAcademicPeriodDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.AddAcademicPeriodAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicPeriodDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "ID in URL and body do not match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _service.UpdateAcademicPeriodAsync(id, dto);
                if (updated == null) return NotFound(new { message = "AcademicPeriod not found." });
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
            var ok = await _service.DeleteAcademicPeriodAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

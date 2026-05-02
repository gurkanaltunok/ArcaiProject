using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ArcaiProject.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingController : ControllerBase
    {
        private readonly IBorrowingRecordService _borrowingService;

        public BorrowingController(IBorrowingRecordService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        // Professor endpoints
        [HttpPost("request")]
        [Authorize(Roles = "Professor")]
        public async Task<ActionResult<BorrowingRecordDto>> RequestDocument([FromBody] CreateBorrowingRequestDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var professorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var newRecord = await _borrowingService.RequestDocumentAsync(createDto, professorId);
                return CreatedAtAction(nameof(GetRecordById), new { id = newRecord.Id }, newRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-requests")]
        [Authorize(Roles = "Professor")]
        public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetMyRequests([FromQuery] PagingParameters pagingParameters)
        {
            var professorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var pagedRecords = await _borrowingService.GetMyRequestsAsync(professorId, pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedRecords.Metadata));
            return Ok(pagedRecords);
        }

        // Admin endpoints
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetPendingRequests([FromQuery] PagingParameters pagingParameters)
        {
            var pagedRecords = await _borrowingService.GetAllPendingRequestsAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedRecords.Metadata));
            return Ok(pagedRecords);
        }

        [HttpGet("approved")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetApprovedRequests([FromQuery] PagingParameters pagingParameters)
        {
            var pagedRecords = await _borrowingService.GetAllApprovedRequestsAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedRecords.Metadata));
            return Ok(pagedRecords);
        }

        [HttpGet("borrowed")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetBorrowedDocuments([FromQuery] PagingParameters pagingParameters)
        {
            var pagedRecords = await _borrowingService.GetAllBorrowedDocumentsAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedRecords.Metadata));
            return Ok(pagedRecords);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetAllBorrowingRecords([FromQuery] PagingParameters pagingParameters)
        {
            var pagedRecords = await _borrowingService.GetAllBorrowingRecordsAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedRecords.Metadata));
            return Ok(pagedRecords);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BorrowingRecordDto>> ApproveRequest(int id)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var record = await _borrowingService.ApproveRequestAsync(id, adminId);
            if (record == null) return NotFound(new { message = "Record not found or not in 'Pending' state." });
            return Ok(record);
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BorrowingRecordDto>> RejectRequest(int id)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var record = await _borrowingService.RejectRequestAsync(id, adminId);
            if (record == null) return NotFound(new { message = "Record not found or not in 'Pending' state." });
            return Ok(record);
        }

        [HttpPost("{id}/checkout")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BorrowingRecordDto>> CheckoutDocument(int id)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var record = await _borrowingService.CheckoutDocumentAsync(id, adminId);
            if (record == null) return NotFound(new { message = "Record not found or not in 'Approved' state." });
            return Ok(record);
        }

        [HttpPost("{id}/return")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BorrowingRecordDto>> ReturnDocument(int id)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var record = await _borrowingService.ReturnDocumentAsync(id, adminId);
            if (record == null) return NotFound(new { message = "Record not found or not in 'CheckedOut' state." });
            return Ok(record);
        }

        // Common
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowingRecordDto>> GetRecordById(int id)
        {
            var record = await _borrowingService.GetRecordByIdAsync(id);
            if (record == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Admin" && record.RequesterUser?.Id != userId)
            {
                return Forbid();
            }

            return Ok(record);
        }
    }
}

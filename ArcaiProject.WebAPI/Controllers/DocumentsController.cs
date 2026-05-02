using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ArcaiProject.WebAPI.Controllers
{
    [Authorize] // Tüm endpoint'ler authenticated kullanıcılar için
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // GET: api/Documents
        // Artık ?pageNumber=1&pageSize=10 gibi parametreler alabilir
        // Professor ve Admin her ikisi de erişebilir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments([FromQuery] PagingParameters pagingParameters)
        {
            // Debug: Log received parameters from query string (case-insensitive)
            var documentTypeId = Request.Query["DocumentTypeId"].FirstOrDefault() ?? Request.Query["documentTypeId"].FirstOrDefault();
            var locationId = Request.Query["LocationId"].FirstOrDefault() ?? Request.Query["locationId"].FirstOrDefault();
            var courseId = Request.Query["CourseId"].FirstOrDefault() ?? Request.Query["courseId"].FirstOrDefault();
            var status = Request.Query["Status"].FirstOrDefault() ?? Request.Query["status"].FirstOrDefault();
            var searchTitle = Request.Query["SearchTitle"].FirstOrDefault() ?? Request.Query["searchTitle"].FirstOrDefault();
            var academicPeriodId = Request.Query["AcademicPeriodId"].FirstOrDefault() ?? Request.Query["academicPeriodId"].FirstOrDefault();
            var tagId = Request.Query["TagId"].FirstOrDefault() ?? Request.Query["tagId"].FirstOrDefault();
            
            System.Diagnostics.Debug.WriteLine($"Query string - DocumentTypeId: {documentTypeId}, LocationId: {locationId}, CourseId: {courseId}, Status: {status}, SearchTitle: {searchTitle}, AcademicPeriodId: {academicPeriodId}, TagId: {tagId}");
            System.Diagnostics.Debug.WriteLine($"Model binding - DocumentTypeId: {pagingParameters.DocumentTypeId}, LocationId: {pagingParameters.LocationId}, CourseId: {pagingParameters.CourseId}, Status: {pagingParameters.Status}, SearchTitle: {pagingParameters.SearchTitle}");
            
            // If query string has values but model binding didn't work, manually parse
            if (!string.IsNullOrEmpty(documentTypeId) && int.TryParse(documentTypeId, out var dtId) && dtId > 0)
            {
                pagingParameters.DocumentTypeId = dtId;
                System.Diagnostics.Debug.WriteLine($"Manually set DocumentTypeId to: {dtId}");
            }
            if (!string.IsNullOrEmpty(locationId) && int.TryParse(locationId, out var locId) && locId > 0)
            {
                pagingParameters.LocationId = locId;
            }
            if (!string.IsNullOrEmpty(courseId) && int.TryParse(courseId, out var crsId) && crsId > 0)
            {
                pagingParameters.CourseId = crsId;
            }
            if (!string.IsNullOrEmpty(status))
            {
                pagingParameters.Status = status;
            }
            if (!string.IsNullOrEmpty(searchTitle))
            {
                pagingParameters.SearchTitle = searchTitle;
            }
            if (!string.IsNullOrEmpty(academicPeriodId) && int.TryParse(academicPeriodId, out var apId) && apId > 0)
            {
                pagingParameters.AcademicPeriodId = apId;
            }
            if (!string.IsNullOrEmpty(tagId) && int.TryParse(tagId, out var tgId) && tgId > 0)
            {
                pagingParameters.TagId = tgId;
            }
            
            var pagedDocuments = await _documentService.GetAllDocumentsAsync(pagingParameters);

            // 1. Meta veriyi X-Pagination header'ına ekle (REST API standardı)
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedDocuments.Metadata));

            // 2. Sadece asıl veriyi (listeyi) body olarak döndür
            return Ok(pagedDocuments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> GetDocument(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null) return NotFound();
            return Ok(document);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin belge ekleyebilir
        public async Task<ActionResult<DocumentDto>> PostDocument([FromBody] CreateDocumentDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminIdStr)) return Unauthorized();
            var adminId = int.Parse(adminIdStr);

            var added = await _documentService.AddDocumentAsync(createDto, adminId);
            return CreatedAtAction(nameof(GetDocument), new { id = added.Id }, added);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin belge güncelleyebilir
        public async Task<IActionResult> PutDocument(int id, [FromBody] UpdateDocumentDto updateDto)
        {
            if (id != updateDto.Id) return BadRequest(new { message = "ID in URL and body do not match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _documentService.UpdateDocumentAsync(id, updateDto);
            if (updated == null) return NotFound(new { message = "Document not found." });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin belge silebilir
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var ok = await _documentService.DeleteDocumentAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ArcaiProject.WebAPI.Controllers
{
    /// <summary>
    /// Belge türü yönetimi için API controller (Sadece Admin erişebilir)
    /// </summary>
    [Authorize(Roles = "Admin")] // Sadece 'Admin' rolüne sahip kullanıcılar erişebilir
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentTypesController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypesController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        /// <summary>
        /// Tüm belge türlerini getirir (Sayfalanmış)
        /// </summary>
        /// <returns>Belge türleri listesi</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentTypeDto>>> GetDocumentTypes([FromQuery] PagingParameters pagingParameters)
        {
            var pagedDocumentTypes = await _documentTypeService.GetAllDocumentTypesAsync(pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedDocumentTypes.Metadata));
            return Ok(pagedDocumentTypes);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip belge türünü getirir
        /// </summary>
        /// <param name="id">Belge türü ID'si</param>
        /// <returns>Belge türü</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentTypeDto>> GetDocumentType(int id)
        {
            var documentType = await _documentTypeService.GetDocumentTypeByIdAsync(id);

            if (documentType == null)
            {
                return NotFound();
            }

            return Ok(documentType);
        }

        /// <summary>
        /// Yeni bir belge türü ekler
        /// </summary>
        /// <param name="createDto">Eklenecek belge türü</param>
        /// <returns>Eklenen belge türü</returns>
        [HttpPost]
        public async Task<ActionResult<DocumentTypeDto>> PostDocumentType([FromBody] CreateDocumentTypeDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var addedDocumentType = await _documentTypeService.AddDocumentTypeAsync(createDto);
                return CreatedAtAction(nameof(GetDocumentType), new { id = addedDocumentType.Id }, addedDocumentType);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mevcut bir belge türünü günceller
        /// </summary>
        /// <param name="id">Güncellenecek belge türünün ID'si</param>
        /// <param name="updateDto">Güncellenmiş belge türü bilgileri</param>
        /// <returns>NoContent veya BadRequest</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocumentType(int id, [FromBody] UpdateDocumentTypeDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(new { message = "ID in URL and body do not match." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDocumentType = await _documentTypeService.UpdateDocumentTypeAsync(id, updateDto);

                if (updatedDocumentType == null)
                {
                    return NotFound(new { message = "DocumentType not found." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip belge türünü siler
        /// </summary>
        /// <param name="id">Silinecek belge türünün ID'si</param>
        /// <returns>NoContent veya NotFound</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            var result = await _documentTypeService.DeleteDocumentTypeAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}


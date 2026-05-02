using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.DTOs;
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
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet("my-unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyUnreadNotifications([FromQuery] PagingParameters pagingParameters)
        {
            var userId = GetCurrentUserId();
            var pagedNotifications = await _notificationService.GetUserUnreadNotificationsAsync(userId, pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedNotifications.Metadata));
            return Ok(pagedNotifications);
        }

        [HttpGet("my-all")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyAllNotifications([FromQuery] PagingParameters pagingParameters)
        {
            var userId = GetCurrentUserId();
            var pagedNotifications = await _notificationService.GetUserAllNotificationsAsync(userId, pagingParameters);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedNotifications.Metadata));
            return Ok(pagedNotifications);
        }

        [HttpGet("my-unread-count")]
        public async Task<ActionResult<int>> GetMyUnreadNotificationsCount()
        {
            var userId = GetCurrentUserId();
            var count = await _notificationService.GetUserUnreadNotificationsCountAsync(userId);
            return Ok(count);
        }

        [HttpPost("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkNotificationAsReadAsync(id, userId);
            if (!result)
            {
                return NotFound(new { message = "Notification not found or does not belong to the user." });
            }
            return NoContent();
        }
    }
}

using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArcaiProject.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ArcaiDbContext context, IMapper mapper, ILogger<NotificationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateNotificationAsync(int userId, string message, string? link = null)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                _logger.LogWarning("Notification attempted for non-existent user ID: {userId}", userId);
                return;
            }

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Notification created for User {userId}: {message}", userId, message);
        }

        public async Task<PagedList<NotificationDto>> GetUserUnreadNotificationsAsync(int userId, PagingParameters pagingParameters)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();

            var dtoQuery = query.ProjectTo<NotificationDto>(_mapper.ConfigurationProvider);

            return await PagedList<NotificationDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<PagedList<NotificationDto>> GetUserAllNotificationsAsync(int userId, PagingParameters pagingParameters)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();

            var dtoQuery = query.ProjectTo<NotificationDto>(_mapper.ConfigurationProvider);

            return await PagedList<NotificationDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notification == null)
            {
                return false;
            }
            if (notification.IsRead)
            {
                return true;
            }
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Notification {id} marked as read by user {userId}", notificationId, userId);
            return true;
        }

        public Task<int> GetUserUnreadNotificationsCountAsync(int userId)
        {
            return _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}

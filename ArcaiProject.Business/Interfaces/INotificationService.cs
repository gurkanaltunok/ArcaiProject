using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message, string? link = null);
        Task<PagedList<NotificationDto>> GetUserUnreadNotificationsAsync(int userId, PagingParameters pagingParameters);
        Task<PagedList<NotificationDto>> GetUserAllNotificationsAsync(int userId, PagingParameters pagingParameters);
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
        Task<int> GetUserUnreadNotificationsCountAsync(int userId);
    }
}

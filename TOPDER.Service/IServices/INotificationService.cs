using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface INotificationService
    {
        Task<NotificationDto> AddAsync(NotificationDto notificationDto);
        Task<bool> UpdateAsync(NotificationDto notificationDto);
        Task<bool> IsReadAsync(int id);
        Task<bool> IsReadAllAsync(int userId);
        Task<bool> RemoveAsync(int id, int Uid);
        Task<bool> RemoveRangeAsync(int userId);
        Task<NotificationDto> GetItemAsync(int notificationId, int userId);
        Task<PaginatedList<NotificationDto>> GetPagingAsync(int pageNumber, int pageSize, int userId);
    }
}

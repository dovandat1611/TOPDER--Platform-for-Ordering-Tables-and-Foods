using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<NotificationDto> AddAsync(NotificationDto notificationDto)
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            var createNotification = await _notificationRepository.CreateAndReturnAsync(notification);
            if(createNotification != null)
            {
                return _mapper.Map<NotificationDto>(createNotification);
            }
            return null;
        }

        public async Task<NotificationDto> GetItemAsync(int notificationId, int userId)
        {
            var query = await _notificationRepository.GetByIdAsync(notificationId);

            if (query == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy thông báo với Id {notificationId}.");
            }

            if (query.Uid != userId)
            {
                throw new UnauthorizedAccessException($"Thông báo với Id {notificationId} không thuộc về user với Id {userId}.");
            }

            return _mapper.Map<NotificationDto>(query);
        }

        public async Task<PaginatedList<NotificationDto>> GetPagingAsync(int pageNumber, int pageSize, int userId)
        {
            var queryable = await _notificationRepository.QueryableAsync();

            var query = queryable.Where(x => x.Uid == userId).OrderByDescending(x => x.NotificationId);

            var queryDTO = query.Select(r => _mapper.Map<NotificationDto>(r));

            var paginatedDTOs = await PaginatedList<NotificationDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 10
            );
            return paginatedDTOs;
        }

        public async Task<bool> IsReadAllAsync(int userId)
        {
            var queryable = await _notificationRepository.QueryableAsync();
            var notification = await queryable.Where(x => x.Uid == userId)
                .Where(x => x.IsRead == false).ToListAsync();

            foreach (var item in notification)
            {
                var existingNotification = await _notificationRepository.GetByIdAsync(item.NotificationId);
                existingNotification.IsRead = true;
                await _notificationRepository.UpdateAsync(existingNotification);
            }
            return true;
        }

        public async Task<bool> IsReadAsync(int id)
        {   
            var existingNotification = await _notificationRepository.GetByIdAsync(id);
            if (existingNotification == null)
            {
                return false;
            }
            if(existingNotification.IsRead == true)
            {
                return true;
            }
            existingNotification.IsRead = true;
            return await _notificationRepository.UpdateAsync(existingNotification);
        }

        public async Task<bool> RemoveAsync(int id, int Uid)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);

            if (notification == null || notification.Uid != Uid)
            {
                return false;
            }
            return await _notificationRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateAsync(NotificationDto notificationDto)
        {
            var existingNotification = await _notificationRepository.GetByIdAsync(notificationDto.Uid);
            if (existingNotification == null || existingNotification.Uid != notificationDto.Uid)
            {
                return false;
            }
            var notification = _mapper.Map<Notification>(notificationDto);
            return await _notificationRepository.UpdateAsync(notification);
        }

    }
}

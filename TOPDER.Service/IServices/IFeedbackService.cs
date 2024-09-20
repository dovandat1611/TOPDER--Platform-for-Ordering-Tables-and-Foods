using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IFeedbackService
    {
        Task<bool> AddAsync(FeedbackDto feedbackDto);
        Task<bool> UpdateAsync(FeedbackDto feedbackDto);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<FeedbackAdminDto>> GetAdminPagingAsync(int pageNumber, int pageSize);
        Task<PaginatedList<FeedbackAdminDto>> SearchAdminPagingAsync(int pageNumber, int pageSize, int star);
        Task<PaginatedList<FeedbackRestaurantDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<FeedbackRestaurantDto>> SearchRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, int star);
        Task<PaginatedList<FeedbackCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<FeedbackCustomerDto>> SearchCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId, int star);
        Task<PaginatedList<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int pageNumber, int pageSize, int customerId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> AddAsync(FeedbackDto feedbackDto);
        Task<bool> UpdateAsync(FeedbackDto feedbackDto);
        Task<bool> InvisibleAsync(int id);
        Task<FeedbackDto> GetFeedbackAsync(int orderId);
        Task<List<FeedbackAdminDto>> ListAdminPagingAsync();
        Task<List<FeedbackRestaurantDto>> ListRestaurantPagingAsync(int restaurantId);
        Task<List<FeedbackCustomerDto>> ListCustomerPagingAsync(int restaurantId);
        Task<List<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int customerId);
    }
}

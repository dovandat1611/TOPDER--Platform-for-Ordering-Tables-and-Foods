using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IOrderService
    {
        Task<Order> AddAsync(OrderDto orderDto);
        Task<bool> UpdateAsync(OrderDto orderDto); 
        Task<OrderPaidEmail> GetOrderPaid(int orderID); 
        Task<bool> RemoveAsync(int id);
        Task<OrderDto> GetItemAsync(int id, int Uid);
        Task<EmailForOrder> GetEmailForOrderAsync(int orderId, string role);
        Task<bool> UpdateStatusAsync(int orderID, string status);
        Task<bool> UpdateStatusOrderPayment(int orderID, string status); 
        Task<bool> CheckIsFirstOrderAsync(int customerId, int restaurantId);
        Task<PaginatedList<OrderDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, string? status, DateTime? month, DateTime? date);
        Task<PaginatedList<OrderCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId, string? status);
    }
}

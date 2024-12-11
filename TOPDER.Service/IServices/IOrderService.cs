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
        //Task<bool> UpdateAsync(OrderDto orderDto);
        Task<bool> UpdatePaidOrderAsync(OrderDto orderDto);
        Task<bool> UpdatePendingOrdersAsync(MultiStatusOrders multiStatus);
        Task<OrderPaidEmail> GetOrderPaid(int orderID); 
        Task<bool> RemoveAsync(int id);
        Task<OrderDto> GetItemAsync(int id, int Uid);
        Task<EmailForOrder> GetEmailForOrderAsync(int orderId, string role);
        Task<OrderDto> UpdateStatusAsync(int orderID, string status);
        Task<bool> UpdateStatusCancelAsync(int orderID, string status, string cancelReason);
        Task<bool> UpdateStatusOrderPayment(int orderID, string status);
       //Task<bool> UpdateTotalIncomeChangeMenuAsync(int orderID, decimal totalAmount);
        Task<bool> UpdateFoodAmountChangeMenuAsync(int orderID, decimal foodAmount);
        Task<bool> UpdateAddFoodAmountChangeMenuAsync(int orderID, decimal addFoodAmount);
        Task<bool> UpdateTotalPaymentAmountAsync(int orderID, decimal totalPaymentAmount);
        Task<bool> CheckIsFirstOrderAsync(int customerId, int restaurantId);
        Task<bool> CheckIsLoyalCustomerAsync(int customerId, int restaurantId);
        Task<CompleteOrderDto> GetInformationForCompleteAsync(int orderID);
        Task<CancelOrderDto> GetInformationForCancelAsync(int userID, int orderID);
        Task<PaginatedList<OrderRestaurantDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, string? status, DateTime? month, DateTime? date);
        Task<PaginatedList<OrderCustomerDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId, string? status);
        Task<List<OrderDto>> GetAdminPagingAsync();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IOrderService
    {
        Task<bool> AddAsync(OrderDto orderDto);
        Task<bool> UpdateAsync(OrderDto orderDto);
        Task<bool> RemoveAsync(int id);
        Task<OrderDto> GetItemAsync(int id, int Uid);
        Task<PaginatedList<OrderDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<OrderDto>> GetCustomerPagingAsync(int pageNumber, int pageSize, int customerId);
    }
}

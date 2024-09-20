using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IDiscountService
    {
        Task<bool> AddAsync(DiscountDto discountDto);
        Task<bool> UpdateAsync(DiscountDto discountDto);
        Task<bool> RemoveAsync(int id, int restaurantId);
        Task<DiscountDto> GetItemAsync(int id, int restaurantId);
        Task<PaginatedList<DiscountDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<PaginatedList<DiscountDto>> GetAvailableDiscountsAsync(int pageNumber, int pageSize, int restaurantId);
    }
}

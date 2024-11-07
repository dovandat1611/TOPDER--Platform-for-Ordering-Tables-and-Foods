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
        Task<bool> InvisibleAsync(int id, int restaurantId);
        Task<AvailableDiscountDto> GetItemAsync(int id, int restaurantId);
        Task<bool> ActiveAsync(ActiveDiscountDto activeDiscount);
        Task<List<AvailableDiscountDto>> GetRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId);
        Task<List<AvailableDiscountDto>> GetAvailableDiscountsAsync(int restaurantId, int customerId, decimal totalPrice);
    }
}
